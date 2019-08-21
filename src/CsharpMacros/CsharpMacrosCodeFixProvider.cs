using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CleanCoder;
using CsharpMacros.Macros;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace CsharpMacros
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CsharpMacrosCodeFixProvider)), Shared]
    public class CsharpMacrosCodeFixProvider : CodeFixProvider
    {
        private const string title = "Execute macro";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(MacroCodeAnalyzer.DiagnosticId);
        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedDocument: c => MakeUppercaseAsync(context.Document, diagnostic.Location, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private readonly Dictionary<string, ICsharpMacro> registeredMacros = new Dictionary<string, ICsharpMacro>()
        {
            ["properties"] = new PropertiesMacro()
        };

        private async Task<Document> MakeUppercaseAsync(Document document, Location diagnosticLocation, CancellationToken cancellationToken)
        {
            var diagnosticSpan = diagnosticLocation.SourceSpan;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var block = root.FindToken(diagnosticSpan.Start).Parent.FirstAncestorOfType<BlockSyntax>();
            if (block == null)
            {
                return document;
            }

            var newStatements = new List<StatementSyntax>();
            var macroFound = false;
            foreach (var statement in block.Statements)
            {
                var triviaList = statement.GetLeadingTrivia();
                if (Contains(statement.GetLocation(), diagnosticLocation) && TryGetMacroDescriptor(triviaList, out var macroDescriptor) )
                {
                    if (registeredMacros.TryGetValue(macroDescriptor.MacroName, out var macro))
                    {
                        var macroContext = await CreateMacroContext(document, cancellationToken);
                        var newContent = TransformContent(macro, macroDescriptor, macroContext);
                        var syntaxTree = SyntaxFactory.ParseStatement(newContent);
                        newStatements.Add(syntaxTree);
                    }
                    newStatements.Add(statement.WithLeadingTrivia(triviaList.LastOrDefault()));
                    macroFound = true;
                }
                else
                {
                    newStatements.Add(statement);
                }
            }

            if (macroFound)
            {
                var newBlock = block.WithStatements(new SyntaxList<StatementSyntax>(newStatements));
                return await ReplaceNodes(document, block, newBlock.WithAdditionalAnnotations(Formatter.Annotation), cancellationToken);
            }
            
            if (Contains(block.CloseBraceToken.GetLocation(), diagnosticLocation) && TryGetMacroDescriptor(block.CloseBraceToken.LeadingTrivia, out var macroDescriptor1))
            {
                if (registeredMacros.TryGetValue(macroDescriptor1.MacroName, out var macro))
                {
                    var macroContext = await CreateMacroContext(document, cancellationToken);
                    var newContent = TransformContent(macro, macroDescriptor1, macroContext);
                    var syntaxTree = SyntaxFactory.ParseStatement(newContent);
                    var newBlock = block.AddStatements(syntaxTree);
                    newBlock = newBlock.WithCloseBraceToken(block.CloseBraceToken.WithLeadingTrivia(block.CloseBraceToken.LeadingTrivia.LastOrDefault()));
                    return await ReplaceNodes(document, block, newBlock.WithAdditionalAnnotations(Formatter.Annotation), cancellationToken);
                }
            }
            return document;
        }

        private static bool Contains(Location currentStatementLocation, Location diagnosticLocation)
        {
            var lineSpan = currentStatementLocation.GetLineSpan();
            var belongsTo = lineSpan.StartLinePosition.Line > diagnosticLocation.GetLineSpan().StartLinePosition.Line;
            return belongsTo;
        }

        private static string TransformContent(ICsharpMacro macro, MacroDescriptor macroDescriptor, ICsharpMacroContext macroContext)
        {
            var sb = new StringBuilder();
            foreach (var item in macro.ExecuteMacro(macroDescriptor.Param, macroContext))
            {
                var itemTemplate = macroDescriptor.Template;
                foreach (var property in item)
                {
                    itemTemplate = itemTemplate.Replace($"${{{macroDescriptor.VarName}.{property.Key}}}", property.Value);
                }

                sb.Append(itemTemplate);
            }
            return sb.ToString().Replace("//", "");
        }

        private static async Task<CsharpMacroContext> CreateMacroContext(Document document, CancellationToken cancellationToken)
        {
            return new CsharpMacroContext()
            {
                SemanticModel = await document.GetSemanticModelAsync(cancellationToken)
            };
        }

        private bool TryGetMacroDescriptor(SyntaxTriviaList triviaList, out MacroDescriptor descriptor)
        {
            var triviaListWithoutEmptyPrefix =  triviaList.SkipWhile(x => string.IsNullOrWhiteSpace(x.ToString())).ToList();
            var header = triviaListWithoutEmptyPrefix.FirstOrDefault().ToString();
            if (header.Contains("macro(") == false)
            {
                descriptor = null;
                return false;
            }

            var templateLines = triviaListWithoutEmptyPrefix.Skip(1).SkipWhile(x=>x.ToString().Equals("\r\n")).Select(x=>x.ToString());
            var matches = macroHeaderSyntax.Match(header);
            descriptor = new MacroDescriptor()
            {
                MacroName = matches.Groups["macro"].Value,
                Param = matches.Groups["param"].Value,
                VarName = matches.Groups["var"].Value,
                Template = string.Join("", templateLines).TrimEnd(' ')
             };
            return true;
        }

        public static async Task<Document> ReplaceNodes(Document document, SyntaxNode oldNode, SyntaxNode newNode, CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            var newRoot = root.ReplaceNode(oldNode, newNode);
            return document.WithSyntaxRoot(newRoot);
        }

        static readonly Regex macroHeaderSyntax = new Regex("macro\\((?<var>.+?)\\s+in\\s(?<macro>.+?)\\((?<param>.+?)\\)\\)", RegexOptions.Compiled);
    }

    internal static class SyntaxExtensions
    {
        public static T FirstAncestorOfType<T>(this SyntaxNode token) where T : class
        {
            if (token is T)
            {
                return token as T;
            }

            if (token.Parent != null)
            {
                return FirstAncestorOfType<T>(token.Parent);
            }

            return default;
        }
    }
}

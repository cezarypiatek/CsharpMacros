using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CleanCoder;
using CsharpMacros.Filters;
using CsharpMacros.Macros;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
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
                    createChangedDocument: c => ExecuteMacro(context.Document, diagnostic.Location, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private readonly Dictionary<string, ICsharpMacro> registeredMacros = new Dictionary<string, ICsharpMacro>()
        {
            ["properties"] = new PropertiesMacro(),
            ["methods"] = new MethodsMacro(),
            ["values"] = new ValuesMacro(),
            ["implement"] = new ImplementMacro(),
            ["derived"] = new DerivedMacro(),
        };

        private async Task<Document> ExecuteMacro(Document document, Location diagnosticLocation, CancellationToken cancellationToken)
        {
            var diagnosticSpan = diagnosticLocation.SourceSpan;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var syntaxNode = root.FindToken(diagnosticSpan.Start).Parent;

            var block = syntaxNode.FirstAncestorOfType<BlockSyntax>();
            if (block != null)
            {
                return await HandleMacroInsideTheBlock(document, diagnosticLocation, cancellationToken, block);
            }

            var classDeclaration = syntaxNode.FirstAncestorOfType<ClassDeclarationSyntax>();
            if (classDeclaration != null)
            {
                return await HandleMacroInsideClass(document, diagnosticLocation, cancellationToken, classDeclaration);
            }
            return document;
        }

        private async Task<Document> HandleMacroInsideTheBlock(Document document, Location diagnosticLocation, CancellationToken cancellationToken, BlockSyntax block)
        {
            foreach (var statement in block.Statements)
            {
                var triviaList = statement.GetLeadingTrivia();
                if (Contains(statement.GetLocation(), diagnosticLocation) &&
                    TryGetMacroDescriptor(triviaList, out var macroDescriptor, out var leadingTrivia))
                {
                    if (registeredMacros.TryGetValue(macroDescriptor.MacroName, out var macro))
                    {
                        var macroContext = await CreateMacroContext(document, diagnosticLocation, cancellationToken);
                        var newContent = TransformContent(macro, macroDescriptor, macroContext);
                        var syntaxTree = SyntaxFactory.ParseStatement(newContent);
                        var newBlock = block.ReplaceNode(statement, new SyntaxNode[]
                        {
                            syntaxTree.WithLeadingTrivia(leadingTrivia),
                            statement.WithLeadingTrivia(triviaList.LastOrDefault())
                        });
                        return await ReplaceNodes(document, block, newBlock.WithAdditionalAnnotations(Formatter.Annotation),
                            cancellationToken);
                    }
                    return document;
                }
            }

            {
                if (Contains(block.CloseBraceToken.GetLocation(), diagnosticLocation) &&
                    TryGetMacroDescriptor(block.CloseBraceToken.LeadingTrivia, out var macroDescriptor1, out var leadingTrivia))
                {
                    if (registeredMacros.TryGetValue(macroDescriptor1.MacroName, out var macro))
                    {
                        var macroContext = await CreateMacroContext(document, diagnosticLocation, cancellationToken);
                        var newContent = TransformContent(macro, macroDescriptor1, macroContext);
                        var syntaxTree = SyntaxFactory.ParseStatement(newContent);
                        var newBlock = block.AddStatements(syntaxTree.WithLeadingTrivia(leadingTrivia));
                        newBlock = newBlock.WithCloseBraceToken(
                            block.CloseBraceToken.WithLeadingTrivia(block.CloseBraceToken.LeadingTrivia.LastOrDefault()));
                        return await ReplaceNodes(document, block, newBlock.WithAdditionalAnnotations(Formatter.Annotation),
                            cancellationToken);
                    }
                }
            }
            return document;
        }

        private async Task<Document> HandleMacroInsideClass(Document document, Location diagnosticLocation, CancellationToken cancellationToken, ClassDeclarationSyntax block)
        {
            foreach (var statement in block.Members)
            {
                var triviaList = statement.GetLeadingTrivia();
                if (Contains(statement.GetLocation(), diagnosticLocation) &&
                    TryGetMacroDescriptor(triviaList, out var macroDescriptor, out var leadingTrivia))
                {
                    if (registeredMacros.TryGetValue(macroDescriptor.MacroName, out var macro))
                    {
                        var macroContext = await CreateMacroContext(document, diagnosticLocation, cancellationToken);
                        var newContent = TransformContent(macro, macroDescriptor, macroContext);
                        var syntaxTree = SyntaxFactory.ParseSyntaxTree(newContent);
                        var newBlock = block.ReplaceNode(statement, syntaxTree.GetRoot().ChildNodes().Concat(new SyntaxNode[]
                        {
                            statement.WithLeadingTrivia(triviaList.LastOrDefault())
                        }));
                        return await ReplaceNodes(document, block, newBlock.WithAdditionalAnnotations(Formatter.Annotation),
                            cancellationToken);
                    }
                    return document;
                }
            }

            {
                if (Contains(block.CloseBraceToken.GetLocation(), diagnosticLocation) &&
                    TryGetMacroDescriptor(block.CloseBraceToken.LeadingTrivia, out var macroDescriptor1, out var leadingTrivia))
                {
                    if (registeredMacros.TryGetValue(macroDescriptor1.MacroName, out var macro))
                    {
                        var macroContext = await CreateMacroContext(document, diagnosticLocation, cancellationToken);
                        var newContent = TransformContent(macro, macroDescriptor1, macroContext);
                        var syntaxTree = SyntaxFactory.ParseSyntaxTree(newContent);
                        var newBlock =
                            block.AddMembers(syntaxTree.GetRoot().ChildNodes().OfType<MemberDeclarationSyntax>().ToArray());
                        newBlock = newBlock.WithCloseBraceToken(
                            block.CloseBraceToken.WithLeadingTrivia(block.CloseBraceToken.LeadingTrivia.LastOrDefault()));
                        return await ReplaceNodes(document, block, newBlock.WithAdditionalAnnotations(Formatter.Annotation),
                            cancellationToken);
                    }
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

        private static readonly Regex placeholderPattern = new Regex(@"(?:\$\{(?<placeholder>.+?)\})",RegexOptions.Compiled);

        private static readonly Dictionary<string, IPlaceholderFilter> filters = new Dictionary<string, IPlaceholderFilter>()
        {
            ["uppercase"] = new UpperCaseFilter(),
            ["lowercase"] = new LowerCaseFilter(),
            ["camelcase"] = new CamelCaseFilter(),
            ["pascalcase"] = new PascalCaseFilter()
        };

        private static string TransformContent(ICsharpMacro macro, MacroDescriptor macroDescriptor, ICsharpMacroContext macroContext)
        {
            var sb = new StringBuilder();
            foreach (var attributes in macro.ExecuteMacro(macroDescriptor.Param, macroContext))
            {
                var transformedItem = TransformedTemplate(macroDescriptor.Template, attributes);
                sb.Append(transformedItem);
            }
            return sb.ToString().Replace("//", "");
        }

        private static string TransformedTemplate(string template, Dictionary<string, string> attributes)
        {
            return placeholderPattern.Replace(template, (match) =>
            {
                var parts = match.Groups["placeholder"].Value.Split('|');
                var key = parts.First().Trim();
                var filterParts = parts.Skip(1).ToArray();
                if (attributes.ContainsKey(key))
                {
                    return TransformValue(attributes[key], filterParts);
                }
                return "__unknown_attribute__";
            });
        }

        private static string TransformValue(string value, string[] filterParts)
        {
            return filterParts.Aggregate(value, (valueToFilter, next) =>
            {
                var filter = next.Trim();
                if (filters.ContainsKey(filter) && string.IsNullOrWhiteSpace(valueToFilter) == false)
                {
                    return filters[filter].Filter(valueToFilter);
                }
                return filter;
            });
        }

        private static async Task<CsharpMacroContext> CreateMacroContext(Document document, Location macroLocation, CancellationToken cancellationToken)
        {
            return new CsharpMacroContext()
            {
                SemanticModel = await document.GetSemanticModelAsync(cancellationToken),
                Solution = document.Project.Solution,
                MacroLocation =  macroLocation
            };
        }

        private (SyntaxTriviaList beforeMacro, SyntaxTriviaList macro) SplitByMacro(SyntaxTriviaList triviaList)
        {
            for (int i = 1; i <= triviaList.Count; i++)
            {
                if (triviaList[i-1].ToString().Contains("macros."))
                {
                    return (SyntaxTriviaList.Empty.AddRange(triviaList.Take(i-1)), SyntaxTriviaList.Empty.AddRange(triviaList.Skip(i - 1)));
                }
            }

            return (triviaList, SyntaxTriviaList.Empty);
        }

        private bool TryGetMacroDescriptor(SyntaxTriviaList triviaList, out MacroDescriptor descriptor, out SyntaxTriviaList leadingTrivia)
        {
            var (beforeMacro, macroTrivia) = SplitByMacro(triviaList);
            leadingTrivia= beforeMacro;
            if (macroTrivia.Count == 0)
            {
                descriptor = null;
                return false;
            }

            var header = macroTrivia.First().ToString();
            var templateLines = macroTrivia.Skip(1).SkipWhile(x=>x.ToString().Equals("\r\n")).Select(x=>x.ToString());
            var matches = macroHeaderSyntax.Match(header);
            descriptor = new MacroDescriptor()
            {
                MacroName = matches.Groups["macro"].Value.Trim(),
                Param = matches.Groups["param"].Value.Trim(),
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

        static readonly Regex macroHeaderSyntax = new Regex("macros\\.(?<macro>.+?)\\((?<param>.+)\\)", RegexOptions.Compiled);
    }
}

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CsharpMacros
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MacroCodeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CM0001";
        internal static readonly LocalizableString Title = "Macro detected";
        internal static readonly LocalizableString MessageFormat = "Comment contains macro";
        internal const string Category = "MacroCodeAnalyzer Category";

        public static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxTreeAction(AnalyzeComment);
        }

        private void AnalyzeComment(SyntaxTreeAnalysisContext context)
        {
            if (context.Tree.FilePath.EndsWith("AssemblyInfo.cs") || context.Tree.FilePath.EndsWith("Designer.cs") || context.Tree.FilePath.EndsWith(".g.cs") || context.Tree.FilePath.EndsWith("AssemblyAttributes.cs"))
            {
                return;
            }

            SyntaxNode root = context.Tree.GetCompilationUnitRoot(context.CancellationToken);
            var commentNodes = root.DescendantTrivia()
                .Where(node => node.IsKind(SyntaxKind.SingleLineCommentTrivia))
                .ToList();

            if (!commentNodes.Any())
            {
                return;
            }

            foreach (var node in commentNodes)
            {
                string commentText = node.ToFullString();
                if (commentText.Contains("macros."))
                {
                    var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}

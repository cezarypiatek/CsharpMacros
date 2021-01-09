using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using RoslynTestKit;

namespace CsharpMacros.Test.Macros
{
    public abstract class MacroTestFixture : CodeFixTestFixture
    {
        protected sealed override string LanguageName => LanguageNames.CSharp;

        protected sealed override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }

        protected void VerifyMacro(string before, string after)
        {
            TestCodeFix(before, after, MacroCodeAnalyzer.Rule);
        }
    }
}
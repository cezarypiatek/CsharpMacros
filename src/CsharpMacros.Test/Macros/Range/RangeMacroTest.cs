using CleanCoder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynTestKit;

namespace CsharpMacros.Test.Macros.Range
{
    public class RangeMacroTest : CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_range()
        {
            TestCodeFix(TestCases._001_SimpleRange, TestCases._001_SimpleRange_FIXED, MacroCodeAnalyzer.Rule);
        }


        protected override string LanguageName => LanguageNames.CSharp;
        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }
    }
}

using CleanCoder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynTestKit;


namespace CsharpMacros.Test.Macros.Values
{
    class ValuesMacroTest : CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_single_values()
        {
            TestCodeFix(TestCases._001_SingleValues, TestCases._001_SingleValues_FIXED, MacroCodeAnalyzer.Rule);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_tuple_values()
        {
            TestCodeFix(TestCases._002_TupleValues, TestCases._002_TupleValues_FIXED, MacroCodeAnalyzer.Rule);
        }

        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }
    }
}

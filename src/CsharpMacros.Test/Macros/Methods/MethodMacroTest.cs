using CleanCoder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynTestKit;

namespace CsharpMacros.Test.Macros.Methods
{
    public class MethodMacroTest : CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_type_that_own_all_methods()
        {
            TestCodeFix(TestCases._001_ObjectWithOwnMethods, TestCases._001_ObjectWithOwnMethods_FIXED, MacroCodeAnalyzer.Rule);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_type_with_void_methods()
        {
            TestCodeFix(TestCases._002_ObjectWithVoidMethod, TestCases._002_ObjectWithVoidMethod_FIXED, MacroCodeAnalyzer.Rule);
        }
        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }
    }
}
using CleanCoder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynTestKit;

namespace CsharpMacros.Test.Macros.Implement
{
    public class ImplementMacroTest : CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_custom_interface()
        {
            TestCodeFix(TestCases._001_CustomInterface, TestCases._001_CustomInterface_FIXED, MacroCodeAnalyzer.Rule);
        }
        [Test]
        public void should_be_able_to_execute_macro_for_generic_interface()
        {
            TestCodeFix(TestCases._002_GenericInterface, TestCases._002_GenericInterface_FIXED, MacroCodeAnalyzer.Rule);
        }

        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }

        public void Register<T1, T2>()
        {

        }
    }
}

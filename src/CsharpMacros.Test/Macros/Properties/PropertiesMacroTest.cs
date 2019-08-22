using CleanCoder;
using CsharpMacros.Test.Macros.Properties;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynNUnitLight;

namespace CsharpMacros.Test
{
    public class PropertiesMacroTest: CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_type_that_own_all_properties()
        {
            TestCodeFix(TestCases._001_ObjectWithOwnProperties, TestCases._001_ObjectWithOwnProperties_FIXED, MacroCodeAnalyzer.Rule);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_type_that_inherit_properties()
        {
            TestCodeFix(TestCases._002_ObjectWithInheritedProperties, TestCases._002_ObjectWithInheritedProperties_FIXED, MacroCodeAnalyzer.Rule);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_generic_type()
        {
            TestCodeFix(TestCases._003_GemericType, TestCases._003_GemericType_FIXED, MacroCodeAnalyzer.Rule);
        }


        [Test]
        public void should_be_able_to_execute_macro_for_type_with_partial_namespace()
        {
            TestCodeFix(TestCases._004_TypeWithNamespace, TestCases._004_TypeWIthNamespace_FIXED, MacroCodeAnalyzer.Rule);
        }

        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }
    }
}

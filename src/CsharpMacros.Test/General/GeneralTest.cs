using CleanCoder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynNUnitLight;

namespace CsharpMacros.Test.General
{
    public class GeneralTest : CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_add_single_filter()
        {
            TestCodeFix(TestCases._001_WithSingleFilter, TestCases._001_WithSIngleFilter_FIXED, MacroCodeAnalyzer.Rule);
        }

        [Test]
        public void should_be_able_to_add_multiple_filters()
        {
            TestCodeFix(TestCases._002_WithMultipleFilters, TestCases._002_WIthMultipleFIlters_FIXED, MacroCodeAnalyzer.Rule);
        }
        
        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }
    }
}

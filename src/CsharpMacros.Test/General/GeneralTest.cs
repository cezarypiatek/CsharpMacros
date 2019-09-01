using CleanCoder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using NUnit.Framework;
using RoslynTestKit;

namespace CsharpMacros.Test.General
{
    public class GeneralTest : CodeFixTestFixture
    {
        [Test]
        public void should_be_able_to_add_single_filter()
        {
            TestCodeFixAtLine(TestCases._001_WithSingleFilter, TestCases._001_WithSIngleFilter_FIXED, MacroCodeAnalyzer.Rule, 21);
        }

        [Test]
        public void should_be_able_to_add_multiple_filters()
        {
            TestCodeFixAtLine(TestCases._002_WithMultipleFilters, TestCases._002_WIthMultipleFIlters_FIXED, MacroCodeAnalyzer.Rule, 21);
        }

        [Test]
        public void should_be_able_to_execute_macro_below_the_comment()
        {
            TestCodeFixAtLine(TestCases._003_CommentAboveTheMacro, TestCases._003_CommentAboveTheMacro_FIXED, MacroCodeAnalyzer.Rule, 22);
        }

        [Test]
        public void should_be_able_to_execute_macro_when_is_single_expression_in_method()
        {
            TestCodeFixAtLine(TestCases._004_WhenMacroIsSingleExpressionInMethod, TestCases._004_WhenMacroIsSingleExpressionInMethod_FIXED, MacroCodeAnalyzer.Rule, 14);
        }

        [Test]
        public void should_be_able_to_execute_macro_when_is_first_expression_in_method()
        {
            TestCodeFixAtLine(TestCases._005_WhenMacroIsFirstExpressionInMethod, TestCases._005_WhenMacroIsFirstExpressionInMethod_FIXED, MacroCodeAnalyzer.Rule, 14);
        }

        [Test]
        public void should_be_able_to_execute_macro_when_is_last_expression_in_method()
        {
            TestCodeFixAtLine(TestCases._006_WhenMacroIsLastExpressionInMethod, TestCases._006_WhenMacroIsLastExpressionInMethod_FIXED, MacroCodeAnalyzer.Rule, 15);
        }

        [Test]
        public void should_be_able_to_execute_macro_inside_if()
        {
            TestCodeFixAtLine(TestCases._007_WhenMacroIsInsideIf_, TestCases._007_WhenMacroIsInsideIf_FIXED, MacroCodeAnalyzer.Rule, 16);
        }

        [Test]
        public void should_be_able_to_execute_macro_inside_if_without_bracket()
        {
            TestCodeFixAtLine(TestCases._008_WhenMacroIsInsideIfWithoutBracket, TestCases._008_WhenMacroIsInsideIfWithoutBracket_FIXED, MacroCodeAnalyzer.Rule, 15);
        }

        [Test]
        public void should_be_able_to_execute_macro_outside_method()
        {
            TestCodeFixAtLine(TestCases._009_WhenMacroIsOutsideMethod, TestCases._009_WhenMacroIsOutsideMethod_FIXED, MacroCodeAnalyzer.Rule, 5);
        }

        [Test]
        public void should_be_able_to_execute_macro_outside_method_and_nothing_more()
        {
            TestCodeFixAtLine(TestCases._010_WhenMacroIsOutsideMethodAndNothingMore, TestCases._010_WhenMacroIsOutsideMethodAndNothingMore_FIXED, MacroCodeAnalyzer.Rule, 5);
        }
        
        protected override string LanguageName => LanguageNames.CSharp;

        protected override CodeFixProvider CreateProvider()
        {
            return new CsharpMacrosCodeFixProvider();
        }
    }
}

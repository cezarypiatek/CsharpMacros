using NUnit.Framework;


namespace CsharpMacros.Test.Macros.Values
{
    class ValuesMacroTest : MacroTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_single_values()
        {
            VerifyMacro(TestCases._001_SingleValues, TestCases._001_SingleValues_FIXED);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_tuple_values()
        {
            VerifyMacro(TestCases._002_TupleValues, TestCases._002_TupleValues_FIXED);
        }
    }
}

using NUnit.Framework;

namespace CsharpMacros.Test.Macros.Range
{
    public class RangeMacroTest : MacroTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_range()
        {
            VerifyMacro(TestCases._001_SimpleRange, TestCases._001_SimpleRange_FIXED);
            VerifyMacro(TestCases._002_SimpleRangeStep, TestCases._002_SimpleRangeStep_FIXED);
        }
    }
}

using NUnit.Framework;

namespace CsharpMacros.Test.Macros.EnumValues
{
    public class EnumValuesMacroTests : MacroTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_enum()
        {
            VerifyMacro(TestCases._001_EnumValues, TestCases._001_EnumValues_FIXED);
        }
    }
}

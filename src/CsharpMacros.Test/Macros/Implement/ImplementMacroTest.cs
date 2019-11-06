using CleanCoder;
using NUnit.Framework;

namespace CsharpMacros.Test.Macros.Implement
{
    public class ImplementMacroTest : MacroTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_custom_interface()
        {
            VerifyMacro(TestCases._001_CustomInterface, TestCases._001_CustomInterface_FIXED);
        }
        [Test]
        public void should_be_able_to_execute_macro_for_generic_interface()
        {
            VerifyMacro(TestCases._002_GenericInterface, TestCases._002_GenericInterface_FIXED);
        }
    }
}

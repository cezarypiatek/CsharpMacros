using NUnit.Framework;

namespace CsharpMacros.Test.Macros.Methods
{
    public class MethodMacroTest : MacroTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_type_that_own_all_methods()
        {
            VerifyMacro(TestCases._001_ObjectWithOwnMethods, TestCases._001_ObjectWithOwnMethods_FIXED);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_type_with_void_methods()
        {
            VerifyMacro(TestCases._002_ObjectWithVoidMethod, TestCases._002_ObjectWithVoidMethod_FIXED);
        } 
        
        [Test]
        public void should_be_able_to_execute_macro_for_interface_and_get_info_about_signature_parameters_and_return_operators()
        {
            VerifyMacro(TestCases._003_ImplementingProxy, TestCases._003_ImplementingProxy_FIXED);
        }
    }
}
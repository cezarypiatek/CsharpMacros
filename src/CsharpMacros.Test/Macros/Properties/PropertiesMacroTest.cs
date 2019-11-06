using CsharpMacros.Test.Macros;
using CsharpMacros.Test.Macros.Properties;
using NUnit.Framework;

namespace CsharpMacros.Test
{
    public class PropertiesMacroTest: MacroTestFixture
    {
        [Test]
        public void should_be_able_to_execute_macro_for_type_that_own_all_properties()
        {
            VerifyMacro(TestCases._001_ObjectWithOwnProperties, TestCases._001_ObjectWithOwnProperties_FIXED);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_type_that_inherit_properties()
        {
            VerifyMacro(TestCases._002_ObjectWithInheritedProperties, TestCases._002_ObjectWithInheritedProperties_FIXED);
        }

        [Test]
        public void should_be_able_to_execute_macro_for_generic_type()
        {
            VerifyMacro(TestCases._003_GemericType, TestCases._003_GemericType_FIXED);
        }


        [Test]
        public void should_be_able_to_execute_macro_for_type_with_partial_namespace()
        {
            VerifyMacro(TestCases._004_TypeWithNamespace, TestCases._004_TypeWIthNamespace_FIXED);
        }
    }
}

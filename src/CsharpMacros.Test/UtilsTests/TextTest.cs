using CsharpMacros.Filters;
using NUnit.Framework;

namespace CsharpMacros.Test.UtilsTests
{
    public class TextTest
    {
        [Test]
        public void should_be_able_to_convert_to_pascal_case_text_with_different_separators()
        {
            Assert.AreEqual("ThisIsTextAbout7777", "This is && text == about 7777".ToPascalCase());
        }

        [Test]
        public void should_be_able_to_convert_camel_case_to_pascal_case()
        {
            Assert.AreEqual("ThisIsTextAbout7777", "thisIsTextAbout7777".ToPascalCase());
        }



        [Test]
        public void should_be_able_to_convert_to_pascal_case_text_with_only_white_space_separators()
        {
            Assert.AreEqual("ThisIsTextAboutSomething", "This	is text\"about\" something".ToPascalCase());
        }

        [Test]
        public void should_be_able_to_convert_to_pascal_when_everything_is_upper_case()
        {
            Assert.AreEqual("ThisIsTextAboutSomething", "THIS IS TEXT ABOUT SOMETHING".ToPascalCase());
        }

        [Test]
        public void should_be_able_to_convert_to_camel_case_text_with_different_separators()
        {
            Assert.AreEqual("thisIsTextAbout7777", "This is && text == about 7777".ToCamelCase());
        }
    }
}
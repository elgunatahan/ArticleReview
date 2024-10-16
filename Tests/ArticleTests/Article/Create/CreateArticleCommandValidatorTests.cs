using Application.Commands.Article.Create;
using FluentAssertions;
using NUnit.Framework;

namespace ArticleTests.Article.Create
{
    [TestFixture]
    [TestFixture]
    public class CreateArticleCommandValidatorTests
    {
        private CreateArticleCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new CreateArticleCommandValidator();
        }

        [TestCase("", "Test Author", 5, "Test Title", "ArticleContent")]
        [TestCase("Test Content", "", 5, "Test Title", "Author")]
        [TestCase("Test Content", "Test Author", 0, "Test Title", "StarCount")]
        [TestCase("Test Content", "Test Author", 5, "", "Title")]
        public void Should_Have_Error_For_Invalid_Inputs(
            string articleContent, string author, int starCount, string title, string expectedErrorField)
        {
            // Arrange
            var command = new CreateArticleCommand
            {
                ArticleContent = articleContent,
                Author = author,
                PublishDate = DateTime.Now,
                StarCount = starCount,
                Title = title
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == expectedErrorField);
        }

        [Test]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            // Arrange
            var command = new CreateArticleCommand
            {
                ArticleContent = "Test Content",
                Author = "Test Author",
                PublishDate = DateTime.Now,
                StarCount = 5,
                Title = "Test Title"
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}

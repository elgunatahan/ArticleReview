using Application.Commands.Article.Update;
using FluentAssertions;
using NUnit.Framework;

namespace ArticleTests.Article.Update
{
    [TestFixture]
    public class UpdateArticleCommandValidatorTests
    {
        private UpdateArticleCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpdateArticleCommandValidator();
        }

        [TestCase("00000000-0000-0000-0000-000000000000", "Test Content", "Test Author", 5, "Test Title", "Id")]
        [TestCase("a24c2943-7666-44ec-89ef-58657b942e06", "", "Test Author", 5, "Test Title", "ArticleContent")]
        [TestCase("e7b94e6c-edb1-4f9c-b820-e34dafffa6b1", "Test Content", "", 5, "Test Title", "Author")]
        [TestCase("7581c2f3-1e62-40c4-bf49-a4200eaa0b5c", "Test Content", "Test Author", 0, "Test Title", "StarCount")]
        [TestCase("daa38c37-e73a-4eeb-abc4-367107676bb9", "Test Content", "Test Author", 5, "", "Title")]
        public void Should_Have_Error_For_Invalid_Inputs(
            string id, string articleContent, string author, int starCount, string title, string expectedErrorField)
        {
            // Arrange
            var command = new UpdateArticleCommand
            {
                Id = expectedErrorField == "Id" ? Guid.Empty : new Guid(id),
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
            var command = new UpdateArticleCommand
            {
                Id = Guid.NewGuid(),
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

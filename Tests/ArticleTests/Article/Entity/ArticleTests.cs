using FluentAssertions;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
namespace ArticleTests.Article.Entity
{
    [TestFixture]
    public class ArticleTests
    {
        [TestCase("", "Test Author", 5, "Test Title", "ArticleContent is null or white space")]
        [TestCase("Test Content", "", 5, "Test Title", "Author is null or white space")]
        [TestCase("Test Content", "Test Author", 0, "Test Title", "StarCount is below or equal to zero")]
        [TestCase("Test Content", "Test Author", 5, "", "Title is null or white space")]
        public void Constructor_Should_Throw_Exception_For_Invalid_Inputs(
            string articleContent,
            string author,
            int starCount,
            string title,
            string expectedErrorMessage)
        {
            // Act
            Action act = () => new Domain.Entities.Article(articleContent, author, DateTime.Now, starCount, title);

            // Assert
            act.Should().Throw<ValidationException>().WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Constructor_Should_Create_Article_When_Valid_Inputs_Are_Provided()
        {
            // Arrange
            var articleContent = "Test Content";
            var author = "Test Author";
            var publishDate = DateTime.Now;
            var starCount = 5;
            var title = "Test Title";

            // Act
            var article = new Domain.Entities.Article(articleContent, author, publishDate, starCount, title);

            // Assert
            article.ArticleContent.Should().Be(articleContent);
            article.Author.Should().Be(author);
            article.PublishDate.Should().Be(publishDate);
            article.StarCount.Should().Be(starCount);
            article.Title.Should().Be(title);
            article.IsDeleted.Should().BeFalse();
        }

        [TestCase("", "Updated Author", 5, "Updated Title", "ArticleContent is null or white space")]
        [TestCase("Updated Content", "", 5, "Updated Title", "Author is null or white space")]
        [TestCase("Updated Content", "Updated Author", 0, "Updated Title", "StarCount is below or equal to zero")]
        [TestCase("Updated Content", "Updated Author", 5, "", "Title is null or white space")]
        public void Update_Should_Throw_Exception_For_Invalid_Inputs(
            string articleContent,
            string author,
            int starCount,
            string title,
            string expectedErrorMessage)
        {
            // Arrange
            var article = new Domain.Entities.Article("Original Content", "Original Author", DateTime.Now, 5, "Original Title");

            // Act
            Action act = () => article.Update(articleContent, author, DateTime.Now, starCount, title);

            // Assert
            act.Should().Throw<ValidationException>().WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_Should_Update_Article_When_Valid_Inputs_Are_Provided()
        {
            // Arrange
            var article = new Domain.Entities.Article("Original Content", "Original Author", DateTime.Now, 5, "Original Title");

            var updatedContent = "Updated Content";
            var updatedAuthor = "Updated Author";
            var updatedPublishDate = DateTime.Now;
            var updatedStarCount = 10;
            var updatedTitle = "Updated Title";

            // Act
            article.Update(updatedContent, updatedAuthor, updatedPublishDate, updatedStarCount, updatedTitle);

            // Assert
            article.ArticleContent.Should().Be(updatedContent);
            article.Author.Should().Be(updatedAuthor);
            article.PublishDate.Should().Be(updatedPublishDate);
            article.StarCount.Should().Be(updatedStarCount);
            article.Title.Should().Be(updatedTitle);
        }

        [Test]
        public void Delete_Should_Set_IsDeleted_To_True()
        {
            // Arrange
            var article = new Domain.Entities.Article("Content", "Author", DateTime.Now, 5, "Title");

            // Act
            article.Delete();

            // Assert
            article.IsDeleted.Should().BeTrue();
        }
    }
}

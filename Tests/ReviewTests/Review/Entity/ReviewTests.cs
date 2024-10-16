using FluentAssertions;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace ReviewTests.Review.Entity
{
    [TestFixture]
    public class ReviewTests
    {
        [TestCase("", "Reviewer", "ReviewContent", "ArticleId is empty guid")]
        [TestCase("f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "", "ReviewContent", "Reviewer is null or white space")]
        [TestCase("f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "Reviewer", "", "ReviewContent is null or white space")]
        public void Constructor_Should_Throw_ValidationException_For_Invalid_Parameters(string articleId, string reviewer, string reviewContent, string expectedErrorMessage)
        {
            // Arrange
            var parsedArticleId = string.IsNullOrEmpty(articleId) ? Guid.Empty : Guid.Parse(articleId);

            // Act
            Action act = () => new Domain.Entities.Review(parsedArticleId, reviewer, reviewContent);

            // Assert
            act.Should().Throw<ValidationException>().WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Constructor_Should_Create_Review_Successfully_When_Valid_Input_Provided()
        {
            // Arrange
            var articleId = Guid.NewGuid();

            // Act
            var review = new Domain.Entities.Review(articleId, "Reviewer", "ReviewContent");

            // Assert
            review.Should().NotBeNull();
            review.ArticleId.Should().Be(articleId);
            review.Reviewer.Should().Be("Reviewer");
            review.ReviewContent.Should().Be("ReviewContent");
            review.IsDeleted.Should().BeFalse();
        }

        [TestCase("", "Updated Reviewer", "Updated Content", "ArticleId is empty guid")]
        [TestCase("f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "", "Updated Content", "Reviewer is null or white space")]
        [TestCase("f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "Updated Reviewer", "", "ReviewContent is null or white space")]
        public void Update_Should_Throw_ValidationException_For_Invalid_Parameters(string articleId, string reviewer, string reviewContent, string expectedErrorMessage)
        {
            // Arrange
            var review = new Domain.Entities.Review(Guid.NewGuid(), "Reviewer", "ReviewContent");
            var parsedArticleId = string.IsNullOrEmpty(articleId) ? Guid.Empty : Guid.Parse(articleId);

            // Act
            Action act = () => review.Update(parsedArticleId, reviewer, reviewContent);

            // Assert
            act.Should().Throw<ValidationException>().WithMessage(expectedErrorMessage);
        }

        [Test]
        public void Update_Should_Update_Review_Successfully_When_Valid_Input_Provided()
        {
            // Arrange
            var review = new Domain.Entities.Review(Guid.NewGuid(), "Reviewer", "ReviewContent");
            var updatedArticleId = Guid.NewGuid();

            // Act
            review.Update(updatedArticleId, "Updated Reviewer", "Updated Content");

            // Assert
            review.ArticleId.Should().Be(updatedArticleId);
            review.Reviewer.Should().Be("Updated Reviewer");
            review.ReviewContent.Should().Be("Updated Content");
        }

        [Test]
        public void Delete_Should_Mark_Review_As_Deleted()
        {
            // Arrange
            var review = new Domain.Entities.Review(Guid.NewGuid(), "Reviewer", "ReviewContent");

            // Act
            review.Delete();

            // Assert
            review.IsDeleted.Should().BeTrue();
        }
    }
}

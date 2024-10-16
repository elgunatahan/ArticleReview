using Application.Commands.Review.Update;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace ReviewTests.Review.Update
{
    [TestFixture]
    public class UpdateReviewCommandValidatorTests
    {
        private UpdateReviewCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new UpdateReviewCommandValidator();
        }

        [TestCase("", "Reviewer", "f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "ReviewContent")]
        [TestCase("Review Content", "", "f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "Reviewer")]
        [TestCase("Review Content", "Reviewer", "", "ArticleId")]
        public void Should_Have_Error_When_Field_Is_Empty(string reviewContent, string reviewer, string articleId, string expectedErrorField)
        {
            // Arrange
            var command = new UpdateReviewCommand
            {
                ReviewContent = reviewContent,
                Reviewer = reviewer,
                ArticleId = string.IsNullOrEmpty(articleId) ? Guid.Empty : Guid.Parse(articleId),
                Id = Guid.NewGuid()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(expectedErrorField);
        }

        [Test]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var command = new UpdateReviewCommand
            {
                ReviewContent = "Updated review content",
                Reviewer = "Reviewer",
                ArticleId = Guid.NewGuid(),
                Id = Guid.NewGuid()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

using Application.Commands.Review.Create;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace ReviewTests.Review.Create
{
    [TestFixture]
    public class CreateReviewCommandValidatorTests
    {
        private CreateReviewCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new CreateReviewCommandValidator();
        }

        [TestCase("", "John Doe", "f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "ReviewContent")]
        [TestCase("This is a review", "", "f1b9e30e-bdb2-46e8-9515-c2a9e3c2c3d0", "Reviewer")]
        [TestCase("This is a review", "John Doe", "", "ArticleId")]
        public void Should_Have_Error_When_Field_Is_Empty(string reviewContent, string reviewer, string articleId, string expectedErrorField)
        {
            // Arrange
            var command = new CreateReviewCommand
            {
                ReviewContent = reviewContent,
                Reviewer = reviewer,
                ArticleId = string.IsNullOrEmpty(articleId) ? Guid.Empty : Guid.Parse(articleId)
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
            var command = new CreateReviewCommand
            {
                ReviewContent = "This is a review",
                Reviewer = "John Doe",
                ArticleId = Guid.NewGuid()
            };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

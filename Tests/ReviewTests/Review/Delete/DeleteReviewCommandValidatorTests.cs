using Application.Commands.Review.Delete;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace ReviewTests.Review.Delete
{
    [TestFixture]
    public class DeleteReviewCommandValidatorTests
    {
        private DeleteReviewCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new DeleteReviewCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var command = new DeleteReviewCommand { Id = Guid.Empty };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Test]
        public void Should_Not_Have_Error_When_Id_Is_Valid()
        {
            // Arrange
            var command = new DeleteReviewCommand { Id = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }
    }
}

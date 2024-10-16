using Application.Queries.Review.GetById;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace ReviewTests.Review.GetById
{
    [TestFixture]
    public class GetReviewByIdQueryValidatorTests
    {
        private GetReviewByIdQueryValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new GetReviewByIdQueryValidator();
        }

        [Test]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var query = new GetReviewByIdQuery { Id = Guid.Empty };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Test]
        public void Should_Not_Have_Error_When_Id_Is_Valid()
        {
            // Arrange
            var query = new GetReviewByIdQuery { Id = Guid.NewGuid() };

            // Act
            var result = _validator.TestValidate(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

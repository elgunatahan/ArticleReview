using Application.Queries.Article.GetById;
using FluentAssertions;
using NUnit.Framework;

namespace ArticleTests.Article.GetById
{
    [TestFixture]
    public class GetArticleByIdQueryValidatorTests
    {
        private GetArticleByIdQueryValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new GetArticleByIdQueryValidator();
        }

        [Test]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            // Arrange
            var query = new GetArticleByIdQuery { Id = Guid.Empty };

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Test]
        public void Should_Not_Have_Error_When_Id_Is_Valid()
        {
            // Arrange
            var query = new GetArticleByIdQuery { Id = Guid.NewGuid() };

            // Act
            var result = _validator.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}

using Application.Commands.Article.Delete;
using FluentAssertions;
using NUnit.Framework;

namespace ArticleTests.Article.Delete
{
    [TestFixture]
    public class DeleteArticleCommandValidatorTests
    {
        private DeleteArticleCommandValidator _validator;

        [SetUp]
        public void SetUp()
        {
            _validator = new DeleteArticleCommandValidator();
        }

        [Test]
        public void Should_Have_Error_When_Id_Is_Empty()
        {
            var command = new DeleteArticleCommand
            {
                Id = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Test]
        public void Should_Not_Have_Error_When_Id_Is_Valid()
        {
            var command = new DeleteArticleCommand
            {
                Id = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}

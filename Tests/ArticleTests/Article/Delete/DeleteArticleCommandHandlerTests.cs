using Application.Commands.Article.Delete;
using Application.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ArticleTests.Article.Delete
{
    [TestFixture]
    public class DeleteArticleCommandHandlerTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private DeleteArticleCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _handler = new DeleteArticleCommandHandler(_articleRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_Should_Throw_ArticleNotFoundException_When_Article_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteArticleCommand
            {
                Id = Guid.NewGuid()
            };

            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Article)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArticleNotFoundException>()
                .WithMessage($"Article with Id {command.Id} is not found.");
        }

        [Test]
        public async Task Handle_Should_Call_Delete_And_Update_When_Article_Exists()
        {
            // Arrange
            var article = new Domain.Entities.Article("Content", "Author", DateTime.Now, 5, "Title");
            var command = new DeleteArticleCommand
            {
                Id = article.IdentityObject.Id
            };

            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(article);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            article.IsDeleted.Should().BeTrue();
            _articleRepositoryMock.Verify(repo => repo.UpdateAsync(article, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

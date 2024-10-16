using Application.Commands.Article.Update;
using Application.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ArticleTests.Article.Update
{
    [TestFixture]
    public class UpdateArticleCommandHandlerTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private UpdateArticleCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _handler = new UpdateArticleCommandHandler(_articleRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_Should_Throw_ArticleNotFoundException_When_Article_Does_Not_Exist()
        {
            // Arrange
            var command = new UpdateArticleCommand
            {
                Id = Guid.NewGuid(),
                ArticleContent = "Updated Content",
                Author = "Updated Author",
                PublishDate = DateTime.Now,
                StarCount = 5,
                Title = "Updated Title"
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
        public async Task Handle_Should_Call_UpdateAsync_When_Article_Exists()
        {
            // Arrange
            var article = new Domain.Entities.Article("Original Content", "Original Author", DateTime.Now.AddYears(-1), 5, "Original Title");
            var command = new UpdateArticleCommand
            {
                Id = article.IdentityObject.Id,
                ArticleContent = "Updated Content",
                Author = "Updated Author",
                PublishDate = DateTime.Now,
                StarCount = 5,
                Title = "Updated Title"
            };

            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(article);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            article.ArticleContent.Should().Be(command.ArticleContent);
            article.Author.Should().Be(command.Author);
            article.PublishDate.Should().Be(command.PublishDate);
            article.StarCount.Should().Be(command.StarCount);
            article.Title.Should().Be(command.Title);
            _articleRepositoryMock.Verify(repo => repo.UpdateAsync(article, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

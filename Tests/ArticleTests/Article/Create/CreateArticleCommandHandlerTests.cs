using Application.Commands.Article.Create;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ArticleTests.Article.Create
{
    [TestFixture]
    public class CreateArticleCommandHandlerTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private CreateArticleCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _handler = new CreateArticleCommandHandler(_articleRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_Should_Create_Article_And_Return_Id()
        {
            // Arrange
            var command = new CreateArticleCommand
            {
                ArticleContent = "Test Content",
                Author = "Test Author",
                PublishDate = DateTime.Now,
                StarCount = 5,
                Title = "Test Title"
            };

            var article = new Domain.Entities.Article(command.ArticleContent, command.Author, command.PublishDate, command.StarCount, command.Title);
            _articleRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Article>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBe(Guid.Empty);
            _articleRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Article>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task Handle_Should_Call_CreateAsync_Once()
        {
            // Arrange
            var command = new CreateArticleCommand
            {
                ArticleContent = "Test Content",
                Author = "Test Author",
                PublishDate = DateTime.Now,
                StarCount = 5,
                Title = "Test Title"
            };

            _articleRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Article>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _articleRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Article>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

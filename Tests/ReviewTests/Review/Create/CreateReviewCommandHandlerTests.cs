using Application.Commands.Review.Create;
using Application.Exceptions;
using Domain.Interfaces.Proxies;
using Domain.Interfaces.Proxies.Responses;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ReviewTests.Review.Create
{
    [TestFixture]
    public class CreateReviewCommandHandlerTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private Mock<IArticleApiProxy> _articleApiProxyMock;
        private CreateReviewCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _articleApiProxyMock = new Mock<IArticleApiProxy>();
            _handler = new CreateReviewCommandHandler(_reviewRepositoryMock.Object, _articleApiProxyMock.Object);
        }

        [Test]
        public async Task Handle_Should_Throw_ArticleNotExistException_When_Article_Does_Not_Exist()
        {
            // Arrange
            var command = new CreateReviewCommand
            {
                ArticleId = Guid.NewGuid(),
                ReviewContent = "This is a review",
                Reviewer = "John Doe"
            };

            _articleApiProxyMock.Setup(proxy => proxy.GetByIdAsync(command.ArticleId))
                .ReturnsAsync((GetArticleByIdProxyResponse)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArticleNotExistException>()
                .WithMessage($"Article with provided id {command.ArticleId} not exist.");
        }

        [Test]
        public async Task Handle_Should_Create_Review_When_Article_Exists()
        {
            // Arrange
            var command = new CreateReviewCommand
            {
                ArticleId = Guid.NewGuid(),
                ReviewContent = "This is a review",
                Reviewer = "John Doe"
            };

            var articleResponse = new GetArticleByIdProxyResponse
            {
                Id = command.ArticleId,
                Title = "Sample Article",
                Author = "Author",
                PublishDate = DateTime.Now,
                StarCount = 5,
                ArticleContent = "Sample Content",
                IsDeleted = false
            };

            _articleApiProxyMock.Setup(proxy => proxy.GetByIdAsync(command.ArticleId))
                .ReturnsAsync(articleResponse);

            _reviewRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            _reviewRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

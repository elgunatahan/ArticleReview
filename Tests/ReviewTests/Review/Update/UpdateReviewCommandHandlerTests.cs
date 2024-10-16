using Application.Commands.Review.Update;
using Application.Exceptions;
using Domain.Interfaces.Proxies;
using Domain.Interfaces.Proxies.Responses;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ReviewTests.Review.Update
{
    [TestFixture]
    public class UpdateReviewCommandHandlerTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private Mock<IArticleApiProxy> _articleApiProxyMock;
        private UpdateReviewCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _articleApiProxyMock = new Mock<IArticleApiProxy>();
            _handler = new UpdateReviewCommandHandler(_reviewRepositoryMock.Object, _articleApiProxyMock.Object);
        }

        [Test]
        public async Task Handle_Should_Throw_ArticleNotExistException_When_Article_Does_Not_Exist()
        {
            // Arrange
            var command = new UpdateReviewCommand
            {
                Id = Guid.NewGuid(),
                ArticleId = Guid.NewGuid(),
                ReviewContent = "Updated review content",
                Reviewer = "Updated Reviewer"
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
        public async Task Handle_Should_Throw_ReviewNotFoundException_When_Review_Does_Not_Exist()
        {
            // Arrange
            var command = new UpdateReviewCommand
            {
                Id = Guid.NewGuid(),
                ArticleId = Guid.NewGuid(),
                ReviewContent = "Updated review content",
                Reviewer = "Updated Reviewer"
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

            _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Review)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ReviewNotFoundException>()
                .WithMessage($"Review with Id {command.Id} is not found.");
        }

        [Test]
        public async Task Handle_Should_Update_Review_When_Article_And_Review_Exist()
        {
            // Arrange
            var command = new UpdateReviewCommand
            {
                Id = Guid.NewGuid(),
                ArticleId = Guid.NewGuid(),
                ReviewContent = "Updated review content",
                Reviewer = "Updated Reviewer"
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

            var review = new Domain.Entities.Review(command.ArticleId, "Old Reviewer", "Old Content");

            _articleApiProxyMock.Setup(proxy => proxy.GetByIdAsync(command.ArticleId))
                .ReturnsAsync(articleResponse);

            _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            _reviewRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _reviewRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Domain.Entities.Review>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

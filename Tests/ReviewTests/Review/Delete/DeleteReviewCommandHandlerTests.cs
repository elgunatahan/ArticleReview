using Application.Commands.Review.Delete;
using Application.Exceptions;
using Domain.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace ReviewTests.Review.Delete
{
    [TestFixture]
    public class DeleteReviewCommandHandlerTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private DeleteReviewCommandHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _handler = new DeleteReviewCommandHandler(_reviewRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_Should_Throw_ReviewNotFoundException_When_Review_Does_Not_Exist()
        {
            // Arrange
            var command = new DeleteReviewCommand { Id = Guid.NewGuid() };

            _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Review)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ReviewNotFoundException>()
                .WithMessage($"Review with Id {command.Id} is not found.");
        }

        [Test]
        public async Task Handle_Should_Call_UpdateAsync_When_Review_Exists()
        {
            // Arrange
            var command = new DeleteReviewCommand { Id = Guid.NewGuid() };

            var review = new Domain.Entities.Review(command.Id, "Reviewer", "Content");

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

using Application.Exceptions;
using Application.Queries.Review.GetById;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace ReviewTests.Review.GetById
{
    [TestFixture]
    public class GetReviewByIdQueryHandlerTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private Mock<IDistributedCache> _cacheMock;
        private GetReviewByIdQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _cacheMock = new Mock<IDistributedCache>();
            _handler = new GetReviewByIdQueryHandler(_reviewRepositoryMock.Object, _cacheMock.Object);
        }

        [Test]
        public async Task Handle_Should_Return_Cached_Review_If_Exists()
        {
            // Arrange
            var query = new GetReviewByIdQuery { Id = Guid.NewGuid() };
            var cachedRepresentation = new GetReviewByIdRepresentation
            {
                Id = query.Id,
                Reviewer = "Cached Reviewer",
                ReviewContent = "Cached Content"
            };

            var cachedData = JsonSerializer.SerializeToUtf8Bytes(cachedRepresentation);
            _cacheMock.Setup(cache => cache.GetAsync(query.Id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Reviewer.Should().Be("Cached Reviewer");
            result.ReviewContent.Should().Be("Cached Content");
            _reviewRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Throw_ReviewNotFoundException_If_Review_Not_Found()
        {
            // Arrange
            var query = new GetReviewByIdQuery { Id = Guid.NewGuid() };

            _cacheMock.Setup(cache => cache.GetAsync(query.Id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Review)null);

            // Act & Assert
            await FluentActions.Awaiting(() => _handler.Handle(query, CancellationToken.None))
                .Should().ThrowAsync<ReviewNotFoundException>()
                .WithMessage($"Review with id {query.Id} is not found.");
        }

        [Test]
        public async Task Handle_Should_Return_Review_And_Cache_It_If_Not_Cached()
        {
            // Arrange
            var query = new GetReviewByIdQuery { Id = Guid.NewGuid() };

            var review = new Domain.Entities.Review(
                query.Id,
                "Test Reviewer",
                "Test ReviewContent",
                false,
                new Domain.ValueObjects.IdentityValueObject(Guid.NewGuid(), 1),
                new Domain.ValueObjects.AuditValueObject("CreatedBy", DateTime.Now, null, null)
            );

            _cacheMock.Setup(cache => cache.GetAsync(query.Id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _reviewRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(review);

            _cacheMock.Setup(cache => cache.SetAsync(query.Id.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Reviewer.Should().Be("Test Reviewer");
            result.ReviewContent.Should().Be("Test ReviewContent");
            _cacheMock.Verify(cache => cache.SetAsync(query.Id.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

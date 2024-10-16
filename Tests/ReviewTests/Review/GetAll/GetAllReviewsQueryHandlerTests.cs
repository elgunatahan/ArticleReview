using Application.Queries.Review.GetAll;
using Domain.Dtos;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace ReviewTests.Review.GetAll
{
    [TestFixture]
    public class GetAllReviewsQueryHandlerTests
    {
        private Mock<IReviewRepository> _reviewRepositoryMock;
        private Mock<IDistributedCache> _cacheMock;
        private GetAllReviewsQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _cacheMock = new Mock<IDistributedCache>();
            _handler = new GetAllReviewsQueryHandler(_cacheMock.Object, _reviewRepositoryMock.Object);
        }

        [Test]
        public async Task Handle_Should_Return_Cached_Reviews_If_Exists()
        {
            // Arrange
            var query = new GetAllReviewsQuery
            {
                QueryOptions = null
            };

            var cachedReviews = new List<ReviewDto>
        {
            new ReviewDto { Id = Guid.NewGuid(), ReviewContent = "Cached Review 1" },
            new ReviewDto { Id = Guid.NewGuid(), ReviewContent = "Cached Review 2" }
        };

            var serializedReviews = JsonSerializer.SerializeToUtf8Bytes(cachedReviews);

            _cacheMock.Setup(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serializedReviews);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().ReviewContent.Should().Be("Cached Review 1");
            _reviewRepositoryMock.Verify(repo => repo.GetQueryable(), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Return_Reviews_From_Repository_And_Cache_It_If_Not_Cached()
        {
            // Arrange
            var query = new GetAllReviewsQuery
            {
                QueryOptions = null
            };

            var reviewsFromRepo = new List<ReviewDto>
        {
            new ReviewDto { Id = Guid.NewGuid(), ReviewContent = "Repo Review 1" },
            new ReviewDto { Id = Guid.NewGuid(), ReviewContent = "Repo Review 2" }
        }.AsQueryable();

            _cacheMock.Setup(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _reviewRepositoryMock.Setup(repo => repo.GetQueryable())
                .Returns(reviewsFromRepo);

            _cacheMock.Setup(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().ReviewContent.Should().Be("Repo Review 1");
            _cacheMock.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

using Application.Exceptions;
using Application.Queries.Article.GetById;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace ArticleTests.Article.GetById
{
    [TestFixture]
    public class GetArticleByIdQueryHandlerTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<IDistributedCache> _cacheMock;
        private GetArticleByIdQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _cacheMock = new Mock<IDistributedCache>();
            _handler = new GetArticleByIdQueryHandler(_articleRepositoryMock.Object, _cacheMock.Object);
        }

        [Test]
        public async Task Handle_Should_Return_Cached_Article_If_Exists()
        {
            // Arrange
            var query = new GetArticleByIdQuery { Id = Guid.NewGuid() };
            var cachedRepresentation = new GetArticleByIdRepresentation
            {
                Id = query.Id,
                Title = "Cached Article",
                Author = "Cached Author",
                StarCount = 5
            };

            var cachedData = JsonSerializer.SerializeToUtf8Bytes(cachedRepresentation);
            _cacheMock.Setup(cache => cache.GetAsync(query.Id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Cached Article");
            result.Author.Should().Be("Cached Author");
            _articleRepositoryMock.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }


        [Test]
        public async Task Handle_Should_Throw_ArticleNotFoundException_If_Article_Not_Found()
        {
            // Arrange
            var query = new GetArticleByIdQuery { Id = Guid.NewGuid() };

            _cacheMock.Setup(cache => cache.GetAsync(query.Id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Domain.Entities.Article)null);

            // Act & Assert
            await FluentActions.Awaiting(() => _handler.Handle(query, CancellationToken.None))
                .Should().ThrowAsync<ArticleNotFoundException>()
                .WithMessage($"Article with Id {query.Id} is not found.");
        }

        [Test]
        public async Task Handle_Should_Return_Article_And_Cache_It_If_Not_Cached()
        {
            // Arrange
            var query = new GetArticleByIdQuery { Id = Guid.NewGuid() };

            var article = new Domain.Entities.Article(
                "Test Content",
                "Test Author",
                DateTime.Now,
                5,
                "Test Title",
                audit: new Domain.ValueObjects.AuditValueObject("System", DateTime.Now, null, null)
            );

            _cacheMock.Setup(cache => cache.GetAsync(query.Id.ToString(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _articleRepositoryMock.Setup(repo => repo.GetByIdAsync(query.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(article);

            _cacheMock.Setup(cache => cache.SetAsync(query.Id.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Test Title");
            result.Author.Should().Be("Test Author");
            _cacheMock.Verify(cache => cache.SetAsync(query.Id.ToString(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

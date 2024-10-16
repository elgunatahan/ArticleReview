using Application.Queries.Article.GetAll;
using Domain.Dtos;
using Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using NUnit.Framework;
using System.Text.Json;

namespace ArticleTests.Article.GetAll
{
    [TestFixture]
    public class GetAllArticlesQueryHandlerTests
    {
        private Mock<IArticleRepository> _articleRepositoryMock;
        private Mock<IDistributedCache> _cacheMock;
        private GetAllArticlesQueryHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _articleRepositoryMock = new Mock<IArticleRepository>();
            _cacheMock = new Mock<IDistributedCache>();
            _handler = new GetAllArticlesQueryHandler(_articleRepositoryMock.Object, _cacheMock.Object);
        }

        [Test]
        public async Task Handle_Should_Return_Cached_Articles_If_Exists()
        {
            // Arrange
            var query = new GetAllArticlesQuery
            {
                QueryOptions = null // OData sorgu seçenekleri kullanılmıyor
            };

            var cachedArticles = new List<ArticleDto>
            {
                new ArticleDto { Id = Guid.NewGuid(), Title = "Cached Article 1" },
                new ArticleDto { Id = Guid.NewGuid(), Title = "Cached Article 2" }
            };

            var serializedArticles = JsonSerializer.SerializeToUtf8Bytes(cachedArticles);

            _cacheMock.Setup(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(serializedArticles);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Title.Should().Be("Cached Article 1");
            _articleRepositoryMock.Verify(repo => repo.GetQueryable(), Times.Never);
        }

        [Test]
        public async Task Handle_Should_Return_Articles_From_Repository_And_Cache_It_If_Not_Cached()
        {
            // Arrange
            var query = new GetAllArticlesQuery
            {
                QueryOptions = null
            };

            var articlesFromRepo = new List<ArticleDto>
            {
                new ArticleDto { Id = Guid.NewGuid(), Title = "Repo Article 1" },
                new ArticleDto { Id = Guid.NewGuid(), Title = "Repo Article 2" }
            }.AsQueryable();

            _cacheMock.Setup(cache => cache.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null);

            _articleRepositoryMock.Setup(repo => repo.GetQueryable())
                .Returns(articlesFromRepo);
            _cacheMock.Setup(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Title.Should().Be("Repo Article 1");
            _cacheMock.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

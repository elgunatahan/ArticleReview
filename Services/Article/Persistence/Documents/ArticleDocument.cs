using Domain.Entities;
using Domain.ValueObjects;
using Persistence.Interfaces;

namespace Persistence.Documents
{
    public class ArticleDocument : IDocument
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }

        public string ArticleContent { get; set; }
        public string Author { get; set; }
        public DateTime PublishDate { get; set; }
        public int StarCount { get; set; }
        public string Title { get; set; }

        public static implicit operator Article(ArticleDocument document)
        {
            if (document == null)
                return null;

            Article article = new Article(
                articleContent: document.ArticleContent,
                author: document.Author,
                publishDate: document.PublishDate,
                starCount: document.StarCount,
                title: document.Title,
                isDeleted: document.IsDeleted,
                identity: new IdentityValueObject(document.Id, document.Version),
                audit: new AuditValueObject(document.CreatedBy, document.CreatedAt, document.UpdatedBy, document.UpdatedAt));

            return article;
        }

        public static explicit operator ArticleDocument(Article article)
        {
            if (article == null)
                return null;

            return new ArticleDocument()
            {
                Id = article.IdentityObject.Id,
                Version = article.IdentityObject.Version,
                CreatedBy = article.Audit?.CreatedBy,
                UpdatedBy = article.Audit?.UpdatedBy,
                IsDeleted = article.IsDeleted,

                ArticleContent = article.ArticleContent,
                Author = article.Author,
                PublishDate = article.PublishDate,
                StarCount = article.StarCount,
                Title = article.Title,
            };
        }
    }
}

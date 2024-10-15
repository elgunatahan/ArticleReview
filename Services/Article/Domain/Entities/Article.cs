using Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Article : BaseEntity
    {
        public string Author { get; private set; }
        public string ArticleContent { get; private set; }
        public DateTime PublishDate { get; private set; }
        public int StarCount { get; private set; }
        public string Title { get; private set; }
        public bool IsDeleted { get; private set; }

        public Article(
            string articleContent,
            string author,
            DateTime publishDate,
            int starCount,
            string title,
            bool isDeleted = false,
            IdentityValueObject identity = null,
            AuditValueObject audit = null) : base(identity)
        {
            ArticleContent = articleContent;
            Author = author;
            PublishDate = publishDate;
            StarCount = starCount;
            Title = title;

            IsDeleted = isDeleted;
            Audit = audit;

            if (string.IsNullOrWhiteSpace(Title))
            {
                throw new ValidationException("Title is null or white space");
            }

            if (StarCount <= 0)
            {
                throw new ValidationException("StarCount is below or equal to zero");
            }

            if (string.IsNullOrWhiteSpace(Author))
            {
                throw new ValidationException("Author is null or white space");
            }

            if (string.IsNullOrWhiteSpace(ArticleContent))
            {
                throw new ValidationException("ArticleContent is null or white space");
            }
        }

        public void Update(
            string articleContent,
            string author,
            DateTime publishDate,
            int starCount,
            string title
            )
        {
            ArticleContent = articleContent;
            Author = author;
            PublishDate = publishDate;
            StarCount = starCount;
            Title = title;

            if(string.IsNullOrWhiteSpace(title))
            {
                throw new ValidationException("Title is null or white space");
            }

            if (StarCount <= 0)
            {
                throw new ValidationException("StarCount is below or equal to zero");
            }

            if (string.IsNullOrWhiteSpace(Author))
            {
                throw new ValidationException("Author is null or white space");
            }

            if (string.IsNullOrWhiteSpace(ArticleContent))
            {
                throw new ValidationException("ArticleContent is null or white space");
            }
        }

        public void Delete()
        {
            IsDeleted = true;
        }
    }
}

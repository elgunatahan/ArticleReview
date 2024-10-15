namespace Application.Queries.Article.GetById
{
    public class GetArticleByIdRepresentation
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

        public GetArticleByIdRepresentation()
        {

        }

        public GetArticleByIdRepresentation(Domain.Entities.Article article)
        {
            Id = article.IdentityObject.Id;
            Version = article.IdentityObject.Version;
            CreatedAt = article.Audit.CreatedAt.Value;
            CreatedBy = article.Audit.CreatedBy;
            UpdatedAt = article.Audit.UpdatedAt;
            UpdatedBy = article.Audit.UpdatedBy;

            ArticleContent = article.ArticleContent;
            Author = article.Author;
            PublishDate = article.PublishDate;
            StarCount = article.StarCount;
            Title = article.Title;
        }
    }
}

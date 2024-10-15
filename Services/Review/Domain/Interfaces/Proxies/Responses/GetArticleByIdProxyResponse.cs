namespace Domain.Interfaces.Proxies.Responses
{
    public class GetArticleByIdProxyResponse
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

    }
}

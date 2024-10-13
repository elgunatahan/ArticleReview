namespace Application.Exceptions
{
    public class ArticleNotFoundException : BaseNotFoundException
    {
        public ArticleNotFoundException(Guid id) : base($"Article with id {id} is not found.")
        {
        }
    }
}

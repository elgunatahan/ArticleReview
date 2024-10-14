namespace Application.Exceptions
{
    public class ArticleNotExistException : BaseBadRequestException
    {
        public ArticleNotExistException(Guid id) : base($"Article with provided id {id} not exist.")
        {
        }
    }
}

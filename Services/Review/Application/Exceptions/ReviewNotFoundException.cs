namespace Application.Exceptions
{
    public class ReviewNotFoundException : BaseNotFoundException
    {
        public ReviewNotFoundException(Guid id) : base($"Review with id {id} is not found.")
        {
        }
    }
}

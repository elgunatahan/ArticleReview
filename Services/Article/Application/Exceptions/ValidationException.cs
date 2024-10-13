using FluentValidation.Results;

namespace Application.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new List<string>();
        }

        public List<string> Failures { get; }

        public ValidationException(List<ValidationFailure> failures)
            : this()
        {
            Failures = failures.Select(x => x.ErrorMessage).ToList();
        }
    }
}

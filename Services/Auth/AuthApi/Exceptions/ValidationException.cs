namespace AuthApi.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new List<string>();
        }

        public List<string> Failures { get; }

        public ValidationException(List<string> failures)
            : this()
        {
            Failures = failures;
        }
    }
}

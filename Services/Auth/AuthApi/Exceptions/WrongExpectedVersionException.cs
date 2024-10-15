namespace AuthApi.Exceptions
{
    public class WrongExpectedVersionException : Exception
    {
        public WrongExpectedVersionException(string message) : base(message)
        {

        }
    }
}

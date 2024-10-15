namespace AuthApi.Exceptions
{
    public class AuthenticationFailedException : Exception
    {
        public AuthenticationFailedException() : base($"Invalid username or password.")
        {
        }
    }
}

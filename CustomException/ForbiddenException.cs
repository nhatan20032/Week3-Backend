namespace EFCorePracticeAPI.CustomException
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "You don't have permission.")
        : base(message)
        {
        }
    }
}

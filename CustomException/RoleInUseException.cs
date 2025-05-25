namespace EFCorePracticeAPI.CustomException
{
    public sealed class RoleInUseException : Exception
    {
        public RoleInUseException(string roleName)
            : base($"The role '{roleName}' is currently in use and cannot be deleted.")
        {            
        }
    }
}

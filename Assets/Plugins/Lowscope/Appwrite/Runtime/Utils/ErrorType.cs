namespace Lowscope.AppwritePlugin.Utils
{
	public enum ErrorType
	{
		Success,
		WrongCredentials,
		InvalidEmail,
		Blocked,
		Failed,
		AlreadyLoggedIn,
		NotLoggedIn,
		MissingCredentials,
		ServerBusy,
		NoConnection,
        Timeout,
        AlreadyVerified,
        NoURLSpecified
    }
}
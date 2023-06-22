namespace Common;

public static class DomainErrors
{
    public static class Register
    {
        public static readonly Error UserAlreadyExists = new("Register.UserAlreadyExists", "User already exists.");
    }

    public static class Login
    {
        public static readonly Error UserDoesNotExist =
            new("Login.UserDoesNotExist", "Username or password is incorrect.");
    }
}
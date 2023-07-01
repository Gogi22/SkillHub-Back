using Common;

namespace Identity.API;

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

    public static class GetUser
    {
        public static readonly Error UserDoesNotExist =
            new("GetUser.UserDoesNotExist", "User with the provided Id does not exist.");
    }

    public static class InternalRequest
    {
        public static readonly Error SecretHeaderInvalid = new("InternalRequest.InvalidSecret", "Secret is invalid.");

        public static readonly Error SecretHeaderMissing =
            new("InternalRequest.SecretHeaderMissing", "Secret header was not provided.");
    }
}
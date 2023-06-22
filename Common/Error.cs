namespace Common;

public class Error
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Message { get; set; }
    public string Code { get; set; }
}
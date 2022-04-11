namespace Parlivote.Shared.Models;

public static class ExceptionMessages
{
    public const string INVALID_ID = "Die Id must have a value!";
    public const string INVALID_STRING = "This field must have a value!";
    public const string DATE_TO_SMALLER_THAN_FROM = "Das Datum von darf nicht hinter dem Datum Bis liegen!";

    public static class Motions
    {
        public const string INVALID_VERSION = "The version must be positive";
    }

    public static class Identity
    {
        public const string INVALID_EMAIL = "The E-Mail address has an invalid format!";
    }
}
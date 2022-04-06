namespace Parlivote.Shared.Models;

public static class ExceptionMessages
{
    public const string INVALID_ID = "Die Id ist ein Pflichtfeld!";
    public const string INVALID_STRING = "Text darf nicht leer sein!";
    public const string DATE_TO_SMALLER_THAN_FROM = "Das Datum von darf nicht hinter dem Datum Bis liegen!";

    public static class Motions
    {
        public const string INVALID_VERSION = "Die Version muss positiv sein";
    }
}
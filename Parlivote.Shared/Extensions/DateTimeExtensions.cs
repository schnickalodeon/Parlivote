namespace Parlivote.Shared.Extensions;

public static class DateTimeExtensions
{
    public static bool IsBefore(this DateTime dt, DateTime other)
    {
        return DateTime.Compare(dt, other) < 0;
    }

    public static bool IsAfter(this DateTime dt, DateTime other)
    {
        return DateTime.Compare(dt, other) > 0;
    }
}
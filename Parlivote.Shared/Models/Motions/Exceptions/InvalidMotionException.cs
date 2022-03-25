using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class InvalidMotionException : Xeption
{
    public InvalidMotionException()
    : base("Invalid Motion, please correct the errors and try again!")
    { }
}
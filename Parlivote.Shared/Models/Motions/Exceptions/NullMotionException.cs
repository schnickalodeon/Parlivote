using Xeptions;

namespace Parlivote.Shared.Models.Motions.Exceptions;

public class NullMotionException : Xeption
{
    public NullMotionException()
    : base(message:"Motion cannot be empty!")
    { }
}
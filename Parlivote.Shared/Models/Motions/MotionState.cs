using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Parlivote.Shared.Models.Motions
{
    public static class MotionStateExtensions
    {
        public static string GetValue(this MotionState state) =>
            state switch
            {
                MotionState.Submitted => MotionStateConverter.Submitted,
                MotionState.Pending => MotionStateConverter.Pending,
                MotionState.Accepted => MotionStateConverter.Accepted,
                MotionState.Declined => MotionStateConverter.Declined,
                MotionState.Cancelled => MotionStateConverter.Cancelled,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
    }
    public static class MotionStateConverter
    {
        public const string Submitted = "Eingereicht";
        public const string Pending = "Ausstehend";
        public const string Accepted = "Angenommen";
        public const string Declined = "Abgelehnt";
        public const string Cancelled = "Abgebrochen";

        public static MotionState FromString(string motionState) =>
            motionState switch
                {
                    Submitted => MotionState.Submitted,
                    Pending => MotionState.Pending,
                    Accepted => MotionState.Accepted,
                    Declined => MotionState.Declined,
                    Cancelled => MotionState.Cancelled,
                    _ => throw new ArgumentNullException(nameof(motionState),"Invalid string for state"),
                };
    }
    public enum MotionState
    {
        Submitted,
        Pending,
        Accepted,
        Declined,
        Cancelled,
        Unset
    }
}

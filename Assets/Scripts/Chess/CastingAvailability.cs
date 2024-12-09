using System;

namespace Chess
{
    [Flags]
    public enum CastingAvailability
    {
        None = 0,
        WhiteKingside = 1,
        WhiteQueenside = 2,
        BlackKingside = 4,
        BlackQueenside = 8,
    }
}
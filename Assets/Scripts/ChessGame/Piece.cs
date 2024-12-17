using System;

namespace Chess
{
    [System.Serializable]
    public struct Piece : IEquatable<Piece>
    {
        public PieceColor Color;
        public PieceType Type;
        public bool Equals(Piece other)
        {
            return Color == other.Color && Type == other.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is Piece other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Color, (int)Type);
        }

        
        public Piece(char c)
        {
            if (char.IsUpper(c))
            {
                Color = PieceColor.White;
            }else if (char.IsLower(c)){
                Color = PieceColor.Black;
            }else
            {
                Color = PieceColor.None;
            }

            var l = char.ToLower(c);
            switch (l)
            {
                case 'k':
                    Type = PieceType.King;
                    break;
                case 'q':
                    Type = PieceType.Queen;
                    break;
                case 'r':
                    Type = PieceType.Rook;
                    break;
                case 'p':
                    Type = PieceType.Pawn;
                    break;
                case 'b':
                    Type = PieceType.Bishop;
                    break;
                case 'n':
                    Type = PieceType.Knight;
                    break;
                default:
                    Type = PieceType.None;
                    break;
            }
        }

        public override string ToString()
        {
            var c = Color == PieceColor.White ? "W" : "B";
            return c + "-" + Type.ToString();
        }
    }
    public enum PieceType
    {
        None = 0,
        Pawn,
        Rook,
        Bishop,
        Knight,
        King,
        Queen,
    }

    public enum PieceColor
    {
        None = 0,
        Black,
        White,
    }
    
    
}
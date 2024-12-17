using Chess;
using UnityEngine;

[CreateAssetMenu(fileName = "Chess Sprite Set", menuName = "Chess/Sprite Set", order = 0)]
public class ChessSpriteSet : ScriptableObject
{
    public Sprite BlackKing;
    public Sprite BlackQueen;
    public Sprite BlackRook;
    public Sprite BlackBishop;
    public Sprite BlackKnight;
    public Sprite BlackPawn;
    public Sprite WhiteKing;
    public Sprite WhiteQueen;
    public Sprite WhiteRook;
    public Sprite WhiteBishop;
    public Sprite WhiteKnight;
    public Sprite WhitePawn;

    public Sprite GetSprite(Piece piece)
    {
        if (piece.Equals(null))
        {
            return null;
        }
        
        if (piece.Color == PieceColor.None || piece.Type == PieceType.None)
        {
            return null;
        }
        
        if (piece.Color == PieceColor.Black)
        {
            switch (piece.Type)
            {
                case PieceType.King: return BlackKing;
                case PieceType.Queen: return BlackQueen;
                case PieceType.Rook: return BlackRook;
                case PieceType.Bishop: return BlackBishop;
                case PieceType.Knight: return BlackKnight;
                case PieceType.Pawn: return BlackPawn;
                case PieceType.None: return null;
            }
        }else if (piece.Color == PieceColor.White)
        {
            switch (piece.Type)
            {
                case PieceType.King: return WhiteKing;
                case PieceType.Queen: return WhiteQueen;
                case PieceType.Rook: return WhiteRook;
                case PieceType.Bishop: return WhiteBishop;
                case PieceType.Knight: return WhiteKnight;
                case PieceType.Pawn: return WhitePawn;
                case PieceType.None: return null;
            }
        }

        return null;
    }
}

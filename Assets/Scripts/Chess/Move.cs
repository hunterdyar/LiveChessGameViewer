namespace Chess
{
    public class Move
    {
        public PieceMovement Movement;
        public PieceMovement? CastleMovement = null;
        public ChessPosition? Captured; // You can only capture one per turn, right?
        //... en passant?
        
    }

    public struct PieceMovement
    {
        public ChessPosition Starting;
        public ChessPosition Destination;
    }
}
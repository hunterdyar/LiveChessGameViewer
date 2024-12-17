using Chess;

public interface IRealPieceSubscriber
{
	public void Captured();
	public void Move(ChessPosition newPosition);
	public void Promotion(Piece newPiece);
	public void Destroy();
}

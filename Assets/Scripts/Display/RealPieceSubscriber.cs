using Chess;

public interface IRealPieceSubscriber
{
	public void Init(RealPiece rp, GameViewer2D viewer);
	public void Captured();
	public void Move(ChessPosition newPosition);
	public void Promotion(Piece newPiece);
	public void Destroy();
}

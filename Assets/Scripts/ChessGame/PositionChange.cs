using System;

namespace Chess
{
	public readonly struct PositionChange
	{
		public readonly ChessPosition Position;
		public readonly SquareChange Change;
		public readonly Piece? OldPiece;
		public readonly Piece? Piece;

		public PositionChange(ChessPosition position, Piece? oldPiece,  Piece? piece)
		{
			Position = position;
			Piece = piece;
			OldPiece = oldPiece;

			if (oldPiece.Equals(piece))
			{
				throw new Exception("Piece cannot be the same as old piece.");
			}
			if (oldPiece == null && piece != null)
			{
				Change = SquareChange.Added;
			}else if (oldPiece != null && piece == null)
			{
				Change = SquareChange.Removed;
			}else if (oldPiece != null && piece != null)
			{
				if (oldPiece.Value.Color != piece.Value.Color)
				{
					Change = SquareChange.Captured;
				}
				else
				{
					if (piece.Value.Type != oldPiece.Value.Type)
					{
						Change = SquareChange.Upgrade;
					}
					else
					{
						//catch this at beginning so this check isn't neccesary
						throw new Exception("Pieces are equal");
					}
				}
			}
			else
			{
				throw new Exception("Invalid Change.");
			}
		}
		
	}
}
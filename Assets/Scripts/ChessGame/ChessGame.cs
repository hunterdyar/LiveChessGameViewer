
using System.Collections.Generic;
using Chess;

namespace Chess
{
	public class ChessGame
	{
		private Dictionary<ChessPosition, Piece> _pieces;
		//Black PlayerInfo
		//White PlayerInfo

		//starting board state
		//piece id to position dictionary
		private Piece?[,] _boardState;
		//List of Moves, basically. Each move has every possible thing we could need:
		private Queue<ChessMove> _moves = new Queue<ChessMove>();

		public void Init(string fen)
		{
			_moves.Clear();
			_pieces = new Dictionary<ChessPosition, Piece>();
			_boardState = ChessMove.DecodeBoard(fen);
			//create pieces and place them accordingly.
		}

		public void NextMove(Move move)
		{
			var m = new ChessMove(move);
			m.Calculate(_boardState);
			_moves.Enqueue(m);
		}


	}
}
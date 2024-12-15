using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
	public class ChessMove
	{
		public CastleingAvailability Castleing;
		public PieceColor MoveColor => _moveColor;
		private PieceColor _moveColor;
		private string _enpassantTarget;
		public int HalfmoveClock => _halfmoveClock;
		private int _halfmoveClock;
		public int MoveNumber => _moveNumber;
		private int _moveNumber;
		private string _piecesEncoded;
		private Piece?[,] _board = null;
		private readonly List<PositionChange> _changes = new List<PositionChange>();

		//list of moves
		private List<(ChessPosition oldPos, ChessPosition newPos)> _moves = new List<(ChessPosition, ChessPosition)>();
		//list of captures
		private List<ChessPosition> _captures = new List<ChessPosition>();
		//

		private string givenMove;
		public ChessMove(Move move)
		{
			givenMove = move.LastMove;
			var fen = move.FEN;
			//clear list.
			string[] elements = fen.Split(' ');
			if (elements.Length != 6)
			{
				Debug.LogError($"Fen {fen} is invalid");
			}

			_piecesEncoded = elements[0];
			SetColor(elements[1]);
			SetCastleing(elements[2]);
			_enpassantTarget = elements[3];
			_halfmoveClock = int.Parse(elements[4]);
			_moveNumber = int.Parse(elements[5]);
		}
		
		private void SetCastleing(string e)
        {
            Castleing = 0;
            e = e.Trim();
            if (e == "-" || e == "")
            {
                return;
            }

            if (e.Contains('K'))
            {
                Castleing |= CastleingAvailability.WhiteKingside;
            }
            if (e.Contains('Q'))
            {
                Castleing |= CastleingAvailability.WhiteQueenside;
            }
            if (e.Contains('k'))
            {
                Castleing |= CastleingAvailability.BlackKingside;
            }
            if (e.Contains('q'))
            {
                Castleing |= CastleingAvailability.BlackQueenside;
            }
        }

        private void SetColor(string e)
        {
            e = e.Trim().ToLower();
            if (e == "w")
            {
                _moveColor = PieceColor.White;
            }else if (e == "b")
            {
                _moveColor = PieceColor.Black;
            }
            else
            {
                throw new Exception("Parse Error");
            }
        }

        public void Calculate(Piece?[,] previousBoard)
        {
	        _changes.Clear();
	        
	        //lazy set board.
	        if (_board == null)
	        {
		        _board = DecodeBoard(_piecesEncoded);
	        }
	        
	        //now, we need to discover all of the things that have happened by comparing the board state between current and our decoded.
	        for (int i = 0; i < 8; i++)
	        {
		        for (int j = 0; j < 8; j++)
		        {
			        var c = _board[i, j];
			        var p = previousBoard[i, j];
			        if (c.Equals(p))
			        {
				        continue;
			        }
			        else
			        {
				      _changes.Add(new PositionChange(new ChessPosition(i,j),p,c));
			        }
		        }
	        }
	        //now we know everything that changed. Let's figure out what happened!
	        if (_changes.Count == 0)
	        {
		        return;
	        }

	        if (_changes.Count == 2)
	        {
		        //one might be a capture.
		        if (_changes[0].Change == SquareChange.Captured)
		        {
			        _captures.Add(_changes[0].Position);
		        }else if (_changes[1].Change == SquareChange.Captured)
		        {
			        _captures.Add(_changes[1].Position);
		        }

		        if (_changes[0].Piece.Equals(_changes[1].Piece))
		        {
			        var oldPos = _changes[0].Change == SquareChange.Removed ? _changes[0].Position : _changes[1].Position;
			        var newPos = _changes[0].Change == SquareChange.Added ? _changes[0].Position : _changes[1].Position;
			        if (oldPos.Equals(newPos))
			        {
				        throw new Exception("This isn't a valid move!");
			        }
			        _moves.Add((oldPos,newPos));
		        }
		        else
		        {
			        throw new NotImplementedException("This isn't a valid move!");
		        }
		        
	        }

	        if (_changes.Count == 3)
	        {
		        //en passant! two removed (from and capture) one added.
		        //uh, or castle? If the rook moves to where the king is or vise versa. I'll look it up later.
	        }

	        if (_changes.Count == 4)
	        {
		        //castle, surely?
		        
		        //test if it's not a castle.
		        foreach (var change in _changes)
		        {
			        if (change.Piece.HasValue)
			        {
				        if (change.Piece.Value.Type != PieceType.King || change.Piece.Value.Type != PieceType.Rook)
				        {
					        throw new Exception("This isn't a castle!");
				        }
			        }
		        }
	        }
	        
	        if (_changes.Count == 1)
	        {
		        //we upgraded!
		        if (_changes[0].Change != SquareChange.Upgrade)
		        {
			        throw new Exception("This isn't an upgrade!");
		        }
	        }
	        
	        //todo: Validate our string against the move's string. (givenMove)
        }

        public static Piece?[,] DecodeBoard(string encodedPieces)
        {
	        var board = new Piece?[8, 8];
	        string[] ranks = encodedPieces.Split('/');
	        for (int ri = 0; ri < 8; ri++)
	        {
		        //7-r...
		        //"Each rank is described, starting with rank 8 and ending with rank 1, with a "/" between each one; 
		        int r = 7 - ri;
		        string d = ranks[r];
		        int col = 0;
		        int i = 0;
		        while (i < d.Length)
		        {
			        var c = d[i];
			        if (char.IsDigit(c))
			        {
				        int x = int.Parse(c.ToString());
				        for (int j = 0; j < x; j++)
				        {
					        board[ri, col] = null;
					        col++;
				        }

				        i++;
			        }
			        else
			        {
				        var piece = new Piece(c);

				        board[ri, col] = piece;
				        col++;
				        i++;
			        }
		        }
	        }

	        return board;
        }
	}
}
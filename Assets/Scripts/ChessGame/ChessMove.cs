using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager;
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
		public string FENPieces => _piecesEncoded;
		private string _piecesEncoded;
		private Piece?[,] _board = null;
		private readonly List<PositionChange> _changes = new List<PositionChange>();

		//list of moves
		//todo: these don't need to be lists, we can do it without allocation
		public List<(ChessPosition oldPos, ChessPosition newPos)> Moves => _moves;
		private List<(ChessPosition oldPos, ChessPosition newPos)> _moves = new List<(ChessPosition, ChessPosition)>();
		//list of captures
		public List<ChessPosition> Captures => _captures;
		private List<ChessPosition> _captures = new List<ChessPosition>();

		public List<(ChessPosition Pos, Piece Piece)> Upgrades => _upgrades;
		private List<(ChessPosition, Piece)> _upgrades = new List<(ChessPosition,Piece)>();
		
		private string givenMove;
		public ChessMove(MoveData move)
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

	        var groups = _changes.GroupBy(x => x.Position);
	        if (groups.Max(x => x.Count()) > 1)
	        {
		        throw new Exception("Invalid change set. multiple changes to same square. literally how?");
	        }
	        //now we know everything that changed. Let's figure out what happened!
	        if (_changes.Count == 0)
	        {
		        Debug.LogWarning("Nothing changed. Is this right? could happen during init/first move I think...");
		        return;
	        }

	        if (_changes.Count == 2)
	        {
		        if (_changes[0].Position == _changes[1].Position)
		        {
			        throw new Exception("why didn't the groupby catch this?");
		        }
		        //one might be a capture.
		        if (_changes[0].Change == SquareChange.Captured)
		        {
			        _captures.Add(_changes[0].Position);
		        }
		        
		        if (_changes[1].Change == SquareChange.Captured)
		        {
			        _captures.Add(_changes[1].Position);
		        }

		        if ((_changes[0].Piece.HasValue && _changes[0].Piece.Equals(_changes[1].OldPiece)) ||
		            (_changes[1].Piece.HasValue && _changes[1].Piece.Equals(_changes[0].OldPiece)))
		        {
			        //test agiainst removed becuase other could be "added" OR "captured"
			        var oldPos = _changes[0].Change == SquareChange.Removed ? _changes[0].Position : _changes[1].Position;
			        var newPos = _changes[0].Change == SquareChange.Removed ? _changes[1].Position : _changes[0].Position;
			        if (oldPos.Equals(newPos))
			        {
				        throw new Exception("This isn't a valid move!");
			        }
			        _moves.Add((oldPos,newPos));
		        }
		        else
		        {
			        var pawnFrom = _changes.Where(x => x.Change == SquareChange.Removed && x.OldPiece.HasValue && x.OldPiece.Value.Type == PieceType.Pawn);
			        var upgradeAdd = _changes.Where(x => x.Change == SquareChange.Added && x.Piece.HasValue);//and rank is 0 or 7

			        if (pawnFrom.Count() == 1 && upgradeAdd.Count() == 1)
			        {
				       var pawnChange = pawnFrom.First();
				       var upgrade = upgradeAdd.First();
				        if (pawnChange.OldPiece.Value.Type == upgrade.Piece.Value.Type)
				        {
					        _moves.Add((pawnChange.Position, upgrade.Position));
					        _upgrades.Add((upgrade.Position, upgrade.Piece.Value));
				        }
				        else
				        {
					        throw new NotImplementedException("This isn't a valid move!");
				        }
				        
			        }
		        }
	        }

	        if (_changes.Count == 3)
	        {
		        //en passant! two removed (from and capture) one added.
		        //uh, or castle? If the rook moves to where the king is or vise versa. I'll look it up later.
		        
		        var movedTo = _changes.First(x => x.Change == SquareChange.Added);
		        var movedFrom = _changes.First(x=>x.Change == SquareChange.Removed && x.OldPiece.Equals(movedTo.Piece));
		        if (!movedTo.Piece.HasValue || movedTo.Piece.Value.Type != PieceType.Pawn)
		        {
			        throw new NotImplementedException("This isn't an en passant!");
		        }
		        var captured = _changes.First(x=>x.Change == SquareChange.Removed && !x.OldPiece.Equals(movedFrom.Piece));
		        _captures.Add(captured.Position);
		        _moves.Add((movedFrom.Position,movedTo.Position));
	        }

	        if (_changes.Count == 4)
	        {
		        //castle, surely?
		        
		        //test if it's not a castle.
		        foreach (var change in _changes)
		        {
			        if (change.Piece.HasValue)
			        {
				        var p = change.Piece ?? change.OldPiece;
				        if (p == null || (p.Value.Type != PieceType.King && p.Value.Type != PieceType.Rook))
				        {
					        throw new Exception("This isn't a castle!");
				        }
			        }
		        }
		        
		        var kingOld = _changes.First(x => x.Change == SquareChange.Removed && x.OldPiece.Value.Type == PieceType.King);
		        var kingNew = _changes.First(x => x.Change == SquareChange.Added && x.Piece.Value.Type == PieceType.King);
		        var rookOld = _changes.First(x=>x.Change == SquareChange.Removed && x.OldPiece.Value.Type == PieceType.Rook);
		        var rookNew = _changes.First(x => x.Change == SquareChange.Added && x.Piece.Value.Type == PieceType.Rook);
		        
		        _moves.Add((rookOld.Position,rookNew.Position));
		        _moves.Add((kingOld.Position,kingNew.Position));
	        }
	        
	        if (_changes.Count == 1)
	        {
		        //we upgraded!
		        if (_changes[0].Change != SquareChange.Upgrade)
		        {
			        throw new Exception("This isn't an upgrade!");
		        }
		        _upgrades.Add((_changes[0].Position, _changes[0].Piece.Value));
	        }

	        if (_changes.Count > 4)
	        {
		       // throw new Exception("wtf");
		       Debug.LogWarning("why so many changes");
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
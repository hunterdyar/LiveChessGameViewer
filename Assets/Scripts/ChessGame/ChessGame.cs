
using System;
using System.Collections.Generic;
using Chess;
using UnityEngine;

namespace Chess
{
	public class ChessGame
	{
		public static Action<RealPiece> OnNewRealPiece;
		/// <summary>
		/// Called before RealPieces get updated (and their Move calls).
		/// </summary>
		public static Action OnMoveStart;
		/// <summary>
		/// Called after RealPieces get updated.
		/// </summary>
		public static Action<ChessMove> OnMove;
		//Black PlayerInfo
		//White PlayerInfo

		//starting board state
		//piece id to position dictionary
		public static Action OnNewGameStart;
		public static Action<PieceColor> OnWinner;

		public PieceColor Winner => Winner;
		private PieceColor _winner = PieceColor.None;//todo: track with gamestate not just if winner is set or not
		
		private Piece?[,] _boardState;
		private int _boardStateMoveNumber;
		private int _halfMoveCountWithThisMoveNumber;
		private Dictionary<ChessPosition, RealPiece> _realPieces = new Dictionary<ChessPosition, RealPiece>();
		private List<RealPiece> _capturedPieces = new List<RealPiece>();
		//List of Moves, basically. Each move has every possible thing we could need:
		private Queue<ChessMove> _moves = new Queue<ChessMove>();

		public void Init(Info currentInfo)
		{
			_moves.Clear();
			ClearRealPieces();
			//store information about the players?
			_boardStateMoveNumber = 0;
			_halfMoveCountWithThisMoveNumber = 0;
		}

		private void InitRealPieces(Piece?[,] boardState)
		{
			ClearRealPieces();
			
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 8; j++)
				{
					var p = boardState[i, j];
					if (p.HasValue)
					{
						var pos = new ChessPosition(i, j);
						var rp = new RealPiece(p.Value, pos);
						_realPieces.Add(pos, rp);
						OnNewRealPiece?.Invoke(rp);
					}
				}	
			}
		}

		private void ClearRealPieces()
		{
			//cleanup old game.
			foreach (var rp in _realPieces.Values)
			{
				rp.Destroy();
			}

			_realPieces.Clear();

			foreach (var rp in _capturedPieces)
			{
				rp.Destroy();
			}

			_capturedPieces.Clear();
		}

		public void NextMove(MoveData move)
		{
			var m = new ChessMove(move);
			_moves.Enqueue(m);
		}

		public void Tick()
		{
			//if animating... block ehre.
			//static int for anim count?
			if (_moves.Count > 0)
			{
				//if animating
				var move = _moves.Dequeue();
				if (move.MoveNumber == 1)
				{
					//start of the game!
					_boardState = ChessMove.DecodeBoard(move.FENPieces);
					_boardStateMoveNumber = 1;
					InitRealPieces(_boardState);
				}
				else
				{
					if (move.MoveNumber ==_boardStateMoveNumber)
					{
						_halfMoveCountWithThisMoveNumber++;
						if (_halfMoveCountWithThisMoveNumber > 2)
						{
							Debug.LogError("how many (player) moves this (game) move?");
						}
					}else if (move.MoveNumber - _boardStateMoveNumber != 1)
					{
						if (_boardStateMoveNumber != 0)
						{
							//Reinit???
							Debug.LogWarning(
								$"Did not receive moves in proper sequential order. Did {_boardStateMoveNumber}, got {move.MoveNumber}. Halfcount is {_halfMoveCountWithThisMoveNumber}");
						}

						//todo: call error condition back to manager
						return;
						
					}
					else
					{
						_halfMoveCountWithThisMoveNumber = 0;
					}

					OnMoveStart?.Invoke();
					move.Calculate(_boardState);
					SetStateToMove(move);
					OnMove?.Invoke(move);
					
					//then we ... animate....?
				}
			}
		}

		//Responsible for the _boardState, which we use to calculate move diffs; and for RealPieces, which are what actually matter externally to here.
		private void SetStateToMove(ChessMove move)
		{
			if (_realPieces.Count == 0)
			{
				//this happenes when this is called on like, the exact game end?
				return;
			}
			_boardStateMoveNumber = move.MoveNumber;
			

			foreach (var c in move.Captures)
			{
				if (_boardState[c.Rank, c.File] == null)
				{
					throw new Exception("Can't Capture empty square");
				}
				
				var pos = new ChessPosition(c.Rank, c.File);
				var captured = _realPieces[pos];
				_realPieces.Remove(pos);
				_capturedPieces.Add(captured);
				captured.Capture();
				
				_boardState[c.Rank, c.File] = null;
			}

			foreach (var m in move.Moves)
			{
				var pos = new ChessPosition(m.newPos.Rank, m.newPos.File);
				var old = new ChessPosition(m.oldPos.Rank, m.oldPos.File);
				
				if (_boardState[m.newPos.Rank, m.newPos.File] != null)
				{
					if (_realPieces.TryGetValue(pos, out var rp))
					{
						Debug.LogWarning($"Move but haven't updated to catpure yet! CaptureCount: {move.Captures.Count}");
						rp.Capture();
						_realPieces.Remove(pos);
					}
					else
					{
						Debug.LogWarning("_boardstate and realpiece Dictionary out of sync.");
					}
				}
				
				
				var p = _realPieces[old];
				p.Move(pos);
				//
				_realPieces.Remove(old);
				_realPieces.Add(pos, p);
				//
				_boardState[m.newPos.Rank, m.newPos.File] = _boardState[m.oldPos.Rank, m.oldPos.File];
				_boardState[m.oldPos.Rank, m.oldPos.File] = null;
			}

			foreach (var c in move.Upgrades)
			{
				_boardState[c.Pos.Rank, c.Pos.File] = c.Piece;
				//
				var upgraded = _realPieces[c.Pos];
				upgraded.Promotion(c.Piece);
			}
		}

		public bool DoneDisplaying()
		{
			if (_moves.Count > 0)
			{
				return false;
			}
			return true;
		}

		public void RecreateRealPieces()
		{
			if (_boardState != null)
			{
				InitRealPieces(_boardState);
			}
		}

		public static PieceColor StringToColor(string color)
		{
			switch (color.ToLower())
			{
				case "w":
				case "white":
					return PieceColor.White;
				case "black":
				case "b":
					return PieceColor.Black;
				default:
					throw new Exception($"Unknown Color {color}");
			}
		}

		public void SetWinner(PieceColor winner)
		{
			//set the state to over.
			_winner = winner;
		}
	}
}
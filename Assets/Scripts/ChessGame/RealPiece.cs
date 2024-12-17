using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Chess
{
	public class RealPiece
	{
		public readonly List<IRealPieceSubscriber> Subscribers;
		public Piece Piece;

		public ChessPosition CurrentPosition => Positions.Peek();
		public readonly Stack<ChessPosition> Positions;
		public bool IsCaptured = false;

		public RealPiece(Piece piece, ChessPosition startingPosition)
		{
			Subscribers = new List<IRealPieceSubscriber>();
			Piece = piece;
			//todo: also track what game move we move for scrubbing through history. future thing.
			Positions = new Stack<ChessPosition>();
			Positions.Push(startingPosition);
		}

		public void Subscribe(IRealPieceSubscriber subscriber){
			Subscribers.Add(subscriber);
		}

		public void Unscribe(IRealPieceSubscriber subscriber)
		{
			if (Subscribers.Contains(subscriber))
			{
				Subscribers.Remove(subscriber);
			}
		}
		public void Capture()
		{
			IsCaptured = true;
			foreach (var sub in Subscribers)
			{
				sub.Captured();
			}
		}

		public void Move(ChessPosition newPosition)
		{
			Positions.Push(newPosition);
			foreach (var sub in Subscribers)
			{
				sub.Move(newPosition);
			}
		}

		public void Promotion(Piece newPiece)
		{
			Piece = newPiece;
			foreach (var sub in Subscribers)
			{
				sub.Promotion(Piece);
			}
		}

		public void Destroy()
		{
			foreach (var sub in Subscribers)
			{
				sub.Destroy();
			}
		}
	}
}
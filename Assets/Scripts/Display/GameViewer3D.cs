using System;
using Chess;
using UnityEngine;

namespace DefaultNamespace
{
	public class GameViewer3D : MonoBehaviour
	{
		public ChessModelSet ModelSet;
		[SerializeField] private ModelPieceDisplay piecePrefab;

		public WorldChessGrid WorldGrid;
		private WorldChessGrid _grid;
	
		public PieceAnimation CurrentAnimation = new PieceAnimation();
		private void Awake()
		{
			_grid = GetComponent<WorldChessGrid>();
		}

		private void OnEnable()
		{
			ChessGame.OnNewRealPiece += OnNewRealPiece;
			ChessGame.OnMoveStart += OnMoveStart;
			ChessGame.OnMove += OnMove;
		}
		
		private void OnDisable()
		{
			ChessGame.OnNewRealPiece -= OnNewRealPiece;
			ChessGame.OnMoveStart -= OnMoveStart;
			ChessGame.OnMove -= OnMove;
		}

		private void OnMove(ChessMove cmove)
		{
			//tints...
			CurrentAnimation.Start();
			if (!GameSetings.Animate)
			{
				CurrentAnimation.Complete();
			}
		}
		
		private void OnMoveStart()
		{
			CurrentAnimation.Complete();
			CurrentAnimation.Clear();
		}

		private void OnNewRealPiece(RealPiece realPiece)
		{
			var go = Instantiate(piecePrefab, transform);
			go.Init(realPiece, this);
		}

		private void OnDestroy()
		{
			CurrentAnimation.Clear();
		}
	}
}
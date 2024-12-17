using Chess;
using UnityEngine;

namespace DefaultNamespace
{
	public class InstantSnapSpritePiece : MonoBehaviour, IRealPieceSubscriber
	{
		private SpriteRenderer _renderer;
		public GameViewer2D Viewer;
		private void Awake()
		{
			_renderer = GetComponent<SpriteRenderer>();
		}

		public void Captured()
		{
			_renderer.enabled = false;
		}

		public void Move(ChessPosition newPosition)
		{
			transform.position = Viewer.GetWorldPosition(newPosition);
		}

		public void Promotion(Piece newPiece)
		{
			_renderer.sprite = Viewer.chessSpriteSet.GetSprite(newPiece);
		}

		public void Destroy()
		{
			Destroy(gameObject);
		}
	}
}
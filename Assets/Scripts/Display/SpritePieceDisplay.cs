using Chess;
using UnityEngine;

namespace DefaultNamespace
{
	public class SpritePieceDisplay : MonoBehaviour, IRealPieceSubscriber
	{
		private SpriteRenderer _renderer;
		private GameViewer2D _viewer;

		public void Init(RealPiece rp, GameViewer2D viewer)
		{
			_viewer = viewer;
			rp.Subscribe(this);
			
			//set self to initial rp position. Just using these functions because there is no animation.
			transform.position = _viewer.GetWorldPosition(rp.CurrentPosition);
			Promotion(rp.Piece);
			
		}
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
			var newPos = _viewer.GetWorldPosition(newPosition);
			var p = new PieceAnimation(transform, transform.position, newPos);
			_viewer.StartAnimation(p);
		}

		public void Promotion(Piece newPiece)
		{
			_renderer.sprite = _viewer.chessSpriteSet.GetSprite(newPiece);
		}

		public void Destroy()
		{
			Destroy(gameObject);
		}
	}
}
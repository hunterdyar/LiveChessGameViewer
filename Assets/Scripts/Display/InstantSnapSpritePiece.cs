using Chess;
using UnityEngine;

namespace DefaultNamespace
{
	public class InstantSnapSpritePiece : MonoBehaviour, IRealPieceSubscriber
	{
		private SpriteRenderer _renderer;
		private GameViewer2D _viewer;

		public void Init(RealPiece rp, GameViewer2D viewer)
		{
			_viewer = viewer;
			rp.Subscribe(this);
			
			//set self to initial rp position. Just using these functions because there is no animation.
			Move(rp.CurrentPosition);
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
			transform.position = _viewer.GetWorldPosition(newPosition);
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
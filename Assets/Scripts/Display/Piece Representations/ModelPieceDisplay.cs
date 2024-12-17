using Chess;
using UnityEngine;

namespace DefaultNamespace
{
	public class ModelPieceDisplay : MonoBehaviour, IRealPieceSubscriber
	{
		private MeshRenderer _renderer;
		private GameViewer3D _viewer;
		private GameObject _model;
		public void Init(RealPiece rp, GameViewer3D viewer)
		{
			_viewer = viewer;
			rp.Subscribe(this);

			//set self to initial rp position. Just using these functions because there is no animation.
			transform.position = _viewer.WorldGrid.GetWorldPosition(rp.CurrentPosition);
			var child = _viewer.ModelSet.GetPrefab(rp.Piece);
			_model = Instantiate(child, transform);
			_renderer = _model.GetComponentInChildren<MeshRenderer>();
		}

		public void Captured()
		{
			_viewer.CurrentAnimation.RegisterCaptured(_renderer);
		}

		public void Move(ChessPosition newPosition)
		{
			var newPos = _viewer.WorldGrid.GetWorldPosition(newPosition);
			_viewer.CurrentAnimation.RegisterMovement(transform, transform.position, newPos);
		}

		public void Promotion(Piece newPiece)
		{
			Destroy(_model);
			var child = _viewer.ModelSet.GetPrefab(newPiece);
			_model = Instantiate(child, transform);
			_renderer = _model.GetComponentInChildren<MeshRenderer>();
		}

		public void Destroy()
		{
			Destroy(gameObject);
		}
	}
}
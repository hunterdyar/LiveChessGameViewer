using Chess;
using UnityEngine;

public class SpritePieceDisplay : MonoBehaviour, IRealPieceSubscriber
{
	private SpriteRenderer _renderer;
	private GameViewer2D _viewer;
	private RealPiece _realPiece;

	public static readonly int NormalRenderPriority = 20;
	public void Init(RealPiece rp, GameViewer2D viewer)
	{
		_realPiece = rp;
		_viewer = viewer;
		_realPiece.Subscribe(this);
		
		//set self to initial rp position. Just using these functions because there is no animation.
		transform.position = _viewer.GetWorldPosition(rp.CurrentPosition);
		Promotion(rp.Piece);
		_renderer.rendererPriority =  NormalRenderPriority;

	}
	private void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	public void Captured()
	{
		_renderer.rendererPriority = NormalRenderPriority - 5;
		_viewer.CurrentAnimation.RegisterCaptured(_renderer);
	}

	public void Move(ChessPosition newPosition)
	{
		_renderer.rendererPriority = NormalRenderPriority;
		var newPos = _viewer.GetWorldPosition(newPosition);
		_viewer.CurrentAnimation.RegisterMovement(transform,transform.position,newPos);
	}

	public void Promotion(Piece newPiece)
	{
		_renderer.sprite = _viewer.chessSpriteSet.GetSprite(newPiece);
	}

	public void DoDestroy()
	{
		//unity is weird
		if (this == null)
		{
			return;
		}
		_realPiece.Unsubscribe(this);
		
		Destroy(gameObject);
	}
}

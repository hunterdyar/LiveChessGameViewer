using System;
using Chess;
using JetBrains.Annotations;
using UnityEngine;

public class PieceAnimation
{
		private int pieceCount;

		public Action OnStart;
		public Action OnEnd;
		
		//world space
		private Vector3[] _startPoints;
		private Vector3[] _endPoints;

		[CanBeNull] private Renderer _capturedRenderer;
		
		private readonly Transform[] _elements;
		private Coroutine _routine;
		// private bool _animating => _t is < 1 and > 0;
		private float _t;

		public bool IsComplete => _isComplete;
		private bool _isComplete;

		private const int MaxPieceCount = 5;
		public PieceAnimation()
		{
			_elements = new Transform[MaxPieceCount];
			_startPoints = new Vector3[MaxPieceCount];
			_endPoints = new Vector3[MaxPieceCount];
			pieceCount = 0;
		}

		public void Clear()
		{
			_capturedRenderer = null;
			pieceCount = 0;
		}
		public void Start()
		{
			OnStart?.Invoke();
			_isComplete = false;
			_t = 0;
	
			for (int i = 0; i < pieceCount; i++)
			{
				_elements[i].position = _startPoints[i];
			}
		}

		public void RegisterMovement(Transform t, Vector3 start, Vector3 end)
		{
			_elements[pieceCount] = t;
			_startPoints[pieceCount] = start;
			_endPoints[pieceCount] = end;
			pieceCount++;
		}

		public void RegisterCaptured(Renderer renderer)
		{
			if (_capturedRenderer != null)
			{
				throw new Exception("More than one piece captured during animation");
			}
			_capturedRenderer = renderer;
		}

		public void Tick()
		{
			Tick(Time.deltaTime);
		}
		public void Tick(float delta)
		{
			_t += delta / GameSetings.AnimationMovementDuration;
			if (_t < 1)
			{
				for (var i = 0; i < pieceCount; i++)
				{
					//we weren't supposed to have to this check but pieces destroy themselves and the 
					if (_elements[i] != null)
					{
						_elements[i].position = Vector3.Lerp(_startPoints[i], _endPoints[i], _t);
					}
				}
			}
			else
			{
				Complete();
			}

		}

		public void Complete()
		{
			if (_isComplete)
			{
				return;
			}
			if (_capturedRenderer != null)
			{
				_capturedRenderer.enabled = false;
			}
			
			_t = 1;
			SetToEnd();
			_isComplete = true;
			OnEnd?.Invoke();
		}

		private void SetToEnd()
		{
			for (int i = 0; i < pieceCount; i++)
			{
				if (_elements[i] != null)
				{
					_elements[i].position = _endPoints[i];
				}
			}
		}
	}

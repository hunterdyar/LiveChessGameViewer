﻿using System;
using Chess;
using UnityEngine;

public class PieceAnimation
{
//todo: Initializers with Castling, Captures.
		
		private int pieceCount;

		public Action OnStart;
		public Action OnEnd;
		//world space
		private readonly Vector3[] _startPoints;
		private readonly Vector3[] _endPoints;
		
		private readonly Transform[] _elements;
		private Coroutine _routine;
		// private bool _animating => _t is < 1 and > 0;
		private float _t;
		private float _duration = 0.75f;

		public bool IsComplete => _isComplete;
		private bool _isComplete;

		public PieceAnimation(SpriteRenderer sprite, Vector3 startPos, Vector3 endPos)
		{
			_elements = new[] { sprite.transform };
			_startPoints = new Vector3[] { startPos };
			_endPoints = new Vector3[] { endPos };
			pieceCount = 1;
		}
		//animationCurve
		
		public void Init()
		{
			OnStart?.Invoke();
			_isComplete = false;
			_t = 0;
	
			for (int i = 0; i < pieceCount; i++)
			{
				_elements[i].position = _startPoints[i];
			}
		}

		public void Tick()
		{
			Tick(Time.deltaTime);
		}
		public void Tick(float delta)
		{
			_t += delta / _duration;
			if (_t < 1)
			{
				for (var i = 0; i < pieceCount; i++)
				{
					_elements[i].position = Vector3.Lerp(_startPoints[i],_endPoints[i],_t);
				}
			}
			else
			{
				Complete();
			}

		}

		public void Complete()
		{
			_t = 1;
			SetToEnd();
			_isComplete = true;
			OnEnd?.Invoke();
		}

		private void SetToEnd()
		{
			for (int i = 0; i < pieceCount; i++)
			{
				_elements[i].position = _endPoints[i];
			}
		}
	}
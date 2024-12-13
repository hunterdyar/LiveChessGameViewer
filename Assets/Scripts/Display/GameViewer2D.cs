using System;
using System.Collections;
using System.Collections.Generic;
using Chess;
using UnityEngine;
using UnityEngine.Serialization;

public class GameViewer2D : MonoBehaviour
{
    public Sprite squareSprite;
    [FormerlySerializedAs("brightTint")] public Color brightTile;
    [FormerlySerializedAs("darkTint")] public Color darkTile;
    [FormerlySerializedAs("TintColor")] public Color tintColor;
    public float tintAmount;
    private SpriteRenderer[,] _tiles;
    private SpriteRenderer[,] _pieces;
    public ChessSpriteSet chessSpriteSet;

    private ChessPosition? _lastMoveOld = null;
    private ChessPosition? _lastMoveNew = null;
    
    private readonly Queue<(SquareData[],PieceAnimation)> _pieceAnimationQueue = new Queue<(SquareData[],PieceAnimation)>();
    private PieceAnimation _currentAnimation;
    void Start()
    {
        InitBoard();
    }

    private void OnEnable()
    {
        GameBoard.OnNewMove += OnNewMove;
        GameBoard.OnGameOver += ClearLastTint;
        GameBoard.OnNewGame += ClearLastTint;
    }
    private void OnDisable()
    {
        GameBoard.OnNewMove -= OnNewMove;
    }

    private void OnNewMove(SquareData[] changes, ChessPosition moveOld, ChessPosition moveNew)
    {
        //Create Animation
        var sprite = _pieces[moveNew.File, moveNew.Rank];
        var startPos = _tiles[moveOld.File, moveOld.Rank].transform.position;
        var endPos = sprite.transform.position;

        var anim = new PieceAnimation(sprite, startPos, endPos);

        //Set Tint
        anim.OnStart += () =>
        {
            //reset the tint
            ClearLastTint();
            _lastMoveOld = moveOld;
            SetColor(_tiles[moveOld.File, moveOld.Rank], moveOld.File, moveOld.Rank, tintAmount);
            _lastMoveNew = moveNew;
            SetColor(_tiles[moveNew.File, moveNew.Rank], moveNew.File, moveNew.Rank, tintAmount);
        };
        //finish tint
        anim.OnEnd += ClearLastTint;

        //run the animation! Eventually! 
        _pieceAnimationQueue.Enqueue((changes,anim));
    }

    private void SetSquareByData(SquareData data)
    {
        Sprite sprite = null;
        if (data.Piece.HasValue)
        {
            sprite = chessSpriteSet.GetSprite(data.Piece.Value);
        }

        _pieces[data.Rank,data.File].sprite = sprite;
    }

    private void Update()
    {
        TickCurrentAnimation();
        if (_currentAnimation == null)
        {
            if (_pieceAnimationQueue.Count > 0)
            {
                
                var (sq,anim) = _pieceAnimationQueue.Dequeue();
                _currentAnimation = anim;
                
                //reset to instantly snap to the starting board state.
                foreach (var change in sq)
                {
                    SetSquareByData(change);
                }
                //Unset and animate back to norm.
                _currentAnimation.Init();
                TickCurrentAnimation(); 
                Debug.Log("Start Next Animation");
            }
        }
    }

    private void TickCurrentAnimation()
    {
        if (_currentAnimation != null)
        {
            _currentAnimation.Tick(Time.deltaTime);
            if (_currentAnimation.IsComplete)
            {
                _currentAnimation = null;
            }
        }

    }

    private void ClearLastTint()
    {
        //Reset color to normal
        if (_lastMoveOld != null)
        {
            int r = _lastMoveOld.Value.Rank;
            int f = _lastMoveOld.Value.File;
            SetColor(_tiles[f, r], f, r);
        }

        if (_lastMoveNew != null)
        {
            int r = _lastMoveNew.Value.Rank;
            int f = _lastMoveNew.Value.File;
            SetColor(_tiles[f, r], f, r);
        }
    }

    void InitBoard()
    {
        ClearChildren();
        SpawnBoard();
    }

    private void SpawnBoard()
    {
        _tiles = new SpriteRenderer[8, 8];
        _pieces = new SpriteRenderer[8, 8];
        for (int r = 0; r < 8; r++)
        {
            for (int f = 0; f < 8; f++)
            {
                var s = new GameObject();
                s.name = "Tile " + ChessPosition.XYToRankFile(r, f);
                var sr = s.AddComponent<SpriteRenderer>();
                sr.sprite = squareSprite;
                SetColor(sr,r,f);
                _tiles[r, f] = sr;
                s.transform.SetParent(transform);
                
                //This is the only place where we acknowledge rank/file stored as XY, but viewed as Y X from bottom-left.
                s.transform.localPosition = new Vector3(r, f, 0);
                
                //Put a piece sprite into our engine and leave it empty for now.
                var p = new GameObject();
                p.name = "Piece" + ChessPosition.XYToRankFile(r, f);
                var psr = p.AddComponent<SpriteRenderer>();
                
                psr.sprite = null;
                psr.color = Color.white;
                _pieces[r, f] = psr;
                p.transform.SetParent(transform);
                p.transform.localPosition = new Vector3(r, f, 0);
                psr.sortingOrder = 10;
            }
        }
    }

    private void SetColor(SpriteRenderer sr,int i, int j,float tint = 0)
    {
        sr.color = (i+j) % 2 == 0 ? darkTile : brightTile;
        if (tint != 0)
        {
            sr.color = Color.Lerp(sr.color, tintColor, tint);
        }
    }

    private void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        
    }
}

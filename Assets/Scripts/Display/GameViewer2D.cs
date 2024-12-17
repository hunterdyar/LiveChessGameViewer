using System;
using System.Collections;
using System.Collections.Generic;
using Chess;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class GameViewer2D : MonoBehaviour
{
    public Sprite squareSprite;
    private Sprite tintSprite => squareSprite;
    [FormerlySerializedAs("brightTint")] public Color brightTile;
    [FormerlySerializedAs("darkTint")] public Color darkTile;
    [FormerlySerializedAs("TintColor")] public Color tintColor;
    public float tintAmount;
    private SpriteRenderer[,] _tiles;
    private SpriteRenderer[,] _tints;
    public ChessSpriteSet chessSpriteSet;
    [FormerlySerializedAs("_piecePrefab")] public SpritePieceDisplay piecePrefab;
    
    private ChessPosition? _lastMoveOld = null;
    private ChessPosition? _lastMoveNew = null;
    
    private readonly List<PieceAnimation> _currentAnimations = new List<PieceAnimation>();
    void Start()
    {
        InitBoard();
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

    public void StartAnimation(PieceAnimation animation)
    {
        _currentAnimations.Add(animation);
        animation.Init();
    }

    private void Update()
    {
        foreach (var anim in _currentAnimations)
        {
            if(!anim.IsComplete){
                anim.Tick(Time.deltaTime);
            }
        }
    }

    private void OnMoveStart()
    {
        //Snap current animation to the end. Wait they only just got registered.
        for (int i = 0; i < _currentAnimations.Count; i++)
        {
            _currentAnimations[i].Complete();
        }

        _currentAnimations.Clear();
    }
    private void OnMove(ChessMove cmove)
    {
        ClearLastTint();
        foreach (var move in cmove.Moves)
        {
            _tints[move.oldPos.Rank, move.oldPos.File].enabled = true;
            _tints[move.newPos.Rank, move.newPos.File].enabled = true;
        }

    }
    
    private void OnNewRealPiece(RealPiece rp)
    {
        var go = Instantiate(piecePrefab,transform);
        go.Init(rp,this);
    }

    private void ClearLastTint()
    {
        //todo: do this as an array of like, 4 things.
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                _tints[i, j].enabled = false;
            }
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
        _tints = new SpriteRenderer[8, 8];
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
                p.name = "Tint" + ChessPosition.XYToRankFile(r, f);
                var psr = p.AddComponent<SpriteRenderer>();
                
                psr.sprite = tintSprite;
                psr.color = tintColor;
                psr.enabled = false;
                _tints[r, f] = psr;
                p.transform.SetParent(s.transform);
                p.transform.localPosition = Vector3.zero;
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

    public Vector3 GetWorldPosition(ChessPosition position)
    {
        return new Vector3(position.Rank, position.File, 0);
    }
}

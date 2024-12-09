using System;
using Chess;
using DefaultNamespace;
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

    private (int r, int f) _lastMoveOld = (-1, -1);
    private (int r, int f) _lastMoveNew = (-1, -1);
    void Start()
    {
        InitBoard();
    }

    private void OnEnable()
    {
        GameBoard.OnSquareChanged += OnSquareChanged;
        ChessLiveViewManager.OnNewLastMove += OnNewLastMove;
    }private void OnDisable()
    {
        GameBoard.OnSquareChanged += OnSquareChanged;
        ChessLiveViewManager.OnNewLastMove -= OnNewLastMove;

    }

    private void OnNewLastMove(int ro,int fo, int rn, int fn)
    {
        if (ro >= 8 || fo >= 8 || rn >= 8 || fn >= 8)
        {
            //EHHHH??
            return;
        }
        
        //Reset color to normal
        if (_lastMoveOld.r >= 0 && _lastMoveOld.f >= 0)
        {
            int r = _lastMoveOld.r;
            int f = _lastMoveOld.f;
            SetColor(_tiles[r, f],r, f);
        }

        if (_lastMoveNew.r >= 0 && _lastMoveNew.f >= 0)
        {
            int r = _lastMoveNew.r;
            int f = _lastMoveNew.f;
            SetColor(_tiles[r, f],r, f);
        }
        _lastMoveOld = (ro, fo);
        SetColor(_tiles[ro, fo], ro, fo,tintAmount);
        _lastMoveNew = (rn, fn);
        SetColor(_tiles[rn, fn], rn, fn,tintAmount);
    }

    private void OnSquareChanged(int r, int c, Piece? piece)
    {
        Sprite sprite = null;
        if (piece.HasValue)
        {
            sprite = chessSpriteSet.GetSprite(piece.Value);
        }

        _pieces[c,r].sprite = sprite;
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
                s.name = "Tile " + ChessLiveViewManager.XYToRankFile(r, f);
                var sr = s.AddComponent<SpriteRenderer>();
                sr.sprite = squareSprite;
                SetColor(sr,r,f);
                _tiles[r, f] = sr;
                s.transform.SetParent(transform);
                
                //This is the only place where we acknowledge rank/file stored as XY, but viewed as Y X from bottom-left.
                s.transform.localPosition = new Vector3(r, f, 0);
                
                //Put a piece sprite into our engine and leave it empty for now.
                var p = new GameObject();
                p.name = "Piece" + ChessLiveViewManager.XYToRankFile(r, f);
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

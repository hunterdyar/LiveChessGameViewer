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

    private ChessPosition? _lastMoveOld = null;
    private ChessPosition? _lastMoveNew = null;
    void Start()
    {
        InitBoard();
    }

    private void OnEnable()
    {
        GameBoard.OnSquareChanged += OnSquareChanged;
        GameBoard.OnNewMove += OnNewMove;
        GameBoard.OnGameOver += ClearLastTint;
        GameBoard.OnNewGame += ClearLastTint;
    }
    private void OnDisable()
    {
        GameBoard.OnSquareChanged += OnSquareChanged;
        GameBoard.OnNewMove -= OnNewMove;
    }

    private void OnNewMove(ChessPosition moveOld, ChessPosition moveNew)
    {
        ClearLastTint();

        _lastMoveOld = moveOld;
        SetColor(_tiles[moveOld.File, moveOld.Rank], moveOld.File, moveOld.Rank, tintAmount);
        _lastMoveNew = moveNew;
        SetColor(_tiles[moveNew.File, moveNew.Rank], moveNew.File, moveNew.Rank, tintAmount);
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

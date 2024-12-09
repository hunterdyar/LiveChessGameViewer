using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    [System.Serializable]
    public class GameBoard
    {
        public static Action<int,int,Piece?> OnSquareChanged;
        public string fen;
        public Piece?[,] CurrentBoard = new Piece?[8, 8];
        public Piece?[,] board = new Piece?[8,8];
        public readonly List<Piece> Pieces = new List<Piece>();
        public PieceColor CurrentColor;
        public CastingAvailability Casting;
        private string enpassantTarget;
        private int halfmoveClock;
        public int moveNumber;
        
        public void SetFromFen(string fen)
        {
            //clear list.
            this.fen = fen;
            string[] elements = fen.Split(' ');
            if (elements.Length != 6)
            {
                Debug.LogError($"Fen {fen} is invalid");
            }
            SetPieces(elements[0]);
            SetColor(elements[1]);
            SetCasting(elements[2]);
            enpassantTarget = elements[3];
            halfmoveClock = int.Parse(elements[4]);
            moveNumber = int.Parse(elements[5]);
            UpdateTick();
        }

        public void ResetView()
        {
            CurrentBoard = new Piece?[8, 8];
            //next tick this will invoke all the events?
        }

       /// <summary>
       /// Compare any updates changed since last tick. We can get a bunch of network calls the same frame,
       /// that sort of thing. It's annoyingly common with buffers and hiccups. so more than one piece can be updated.
       /// We don't know what the last move is just by looking at what changed, we use the reported data for that.
       /// </summary>
        public void UpdateTick()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!board[i, j].Equals(CurrentBoard[i, j]))
                    {
                        CurrentBoard[i, j] = board[i, j];
                        OnSquareChanged?.Invoke(i, j, CurrentBoard[i, j]);
                    }
                }
            }
        }
        private void SetCasting(string e)
        {
            Casting = 0;
            e = e.Trim();
            if (e == "-" || e == "")
            {
                return;
            }

            if (e.Contains('K'))
            {
                Casting |= CastingAvailability.WhiteKingside;
            }
            if (e.Contains('Q'))
            {
                Casting |= CastingAvailability.WhiteQueenside;
            }
            if (e.Contains('k'))
            {
                Casting |= CastingAvailability.BlackKingside;
            }
            if (e.Contains('q'))
            {
                Casting |= CastingAvailability.BlackQueenside;
            }
        }

        private void SetColor(string e)
        {
            e = e.Trim().ToLower();
            if (e == "w")
            {
                CurrentColor = PieceColor.White;
            }else if (e == "b")
            {
                CurrentColor = PieceColor.Black;
            }
            else
            {
                throw new Exception("Parse Error");
            }
        }

        private void SetPieces(string element)
        {   
            board = new Piece?[8, 8];
            Pieces.Clear();
            string[] ranks = element.Split('/');
            for (int ri = 0; ri < 8; ri++)
            {
                //7-r...
                //"Each rank is described, starting with rank 8 and ending with rank 1, with a "/" between each one; 
                int r = 7 - ri;
                string d = ranks[r];
                int col = 0;
                int i = 0;
                while (i < d.Length)
                {
                    var c = d[i];
                    if (char.IsDigit(c))
                    {
                        //number of empty spaces.
                        int x= int.Parse(c.ToString());
                        for (int j = 0; j < x; j++)
                        {                            
                            board[ri, col] = null;
                            col++;
                        }
                        i++;
                    }
                    else
                    {
                        var piece = new Piece(c);
                        board[ri,col] = piece;
                        col++;
                        i++;
                    }
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    [System.Serializable]
    public class GameBoard
    {
        public static Action<GameBoard> OnNewGame;
        public static Action OnGameOver;
        public static Action<Move> OnNewMove;
        public string fen;
        public Piece?[,] CurrentBoard = new Piece?[8, 8];
        public Piece?[,] board = new Piece?[8,8];
        public readonly List<Piece> Pieces = new List<Piece>();
        public PieceColor CurrentColor;
        public CastingAvailability Casting;
        private string enpassantTarget;
        private int halfmoveClock;
        public int moveNumber;

        public void CreateNewGame(string fen)
        {
            Debug.Log("New Game");
            SetFromFEN(fen,true);
            OnNewGame?.Invoke(this);
        }

        public void GameOver()
        {
            OnGameOver?.Invoke();
        }
        public void Move(MoveData moveData)
        {
            var actualMove = MoveFromMoveData(moveData);
            OnNewMove?.Invoke(actualMove);
        }

        private void SetFromFEN(string fen, bool hardSetCurrentBoard = false)
        {
            this.fen = fen;
            string[] elements = fen.Split(' ');
            if (elements.Length != 6)
            {
                Debug.LogError($"Fen {fen} is invalid");
            }
            SetPieces(elements[0]);//this sets Board.
            SetColor(elements[1]);
            SetCasting(elements[2]);
            enpassantTarget = elements[3];
            halfmoveClock = int.Parse(elements[4]);
            moveNumber = int.Parse(elements[5]);

            if (hardSetCurrentBoard)
            {
                //todo: array.Copy
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        CurrentBoard[i, j] = board[i,j];
                    }
                }
            }
        }
        private Move MoveFromMoveData(MoveData data)
        {
            //clear list.
            SetFromFEN(data.FEN);

            ChessPosition? captured = null;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!board[i, j].Equals(CurrentBoard[i, j]))
                    {
                        //moving to where a piece is?
                        if (board[i, j] != null && CurrentBoard[i, j] != null)
                        {
                            //upgrading a pawn would be the samecolor
                            if (board[i, j].Value.Color != CurrentBoard[i, j].Value.Color)
                            {
                                captured = new ChessPosition(i, j);
                            }
                            else
                            {
                                Debug.LogError($"Upgrade? Castle? Or its a new game and this is reset code... {data.LastMove}");
                            }
                        }
                        
                        CurrentBoard[i, j] = board[i, j];
                    }
                }
            }
            
            Move m = new Move()
            {
                Movement = new PieceMovement()
                {
                    Starting = data.MoveOldPosition,
                    Destination = data.MoveNewPosition,
                },
                //todo: Castle Movement
                Captured = captured,
            };
            return m;

        }

        public void ResetView()
        {
            CurrentBoard = new Piece?[8, 8];
            //next tick this will invoke all the events?
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
using UnityEditor.Experimental;
using UnityEngine;

namespace Chess
{
    [System.Serializable]
    public class MoveData
    {
        public string FEN => _data.fen;
        public bool HasLastMove => MoveOldPosition.ToString() == MoveNewPosition.ToString();
        public string LastMove => _data.lm;

        public ChessPosition MoveOldPosition;
        public ChessPosition MoveNewPosition;
        
        private MovePacket _data;
        private MoveData() { }
        public MoveData(string rawJSON)
        {
            //Move: {"fen":"r6r/1p3pkp/pQ3np1/3qNp2/3Pn3/7P/PP2NPP1/R2R2K1 b - - 0 20","lm":"e3d4","wc":126,"bc":127}
            _data = JsonUtility.FromJson<MovePacket>(rawJSON);
            SetLastMove(_data.lm);
            //Set board to FEN state minus the Last Move.
            //Animate the move. Move done and animated.
        }

        public override string ToString()
        {
            return _data.lm.ToString();
        }

        private void SetLastMove(string lm)
        {
            if (string.IsNullOrEmpty(lm))
            {
                return;
            }

            if (lm.Length != 4)
            {
                Debug.LogWarning($"Unknown last move {lm}");
                return;
            }

            //1-8 int is the (row) rank
            //a-h char is the (col) file
            MoveOldPosition = new ChessPosition(lm[0], lm[1]);
            MoveNewPosition = new ChessPosition(lm[2], lm[3]);
        }

        /// <summary>
        /// Hacky workaround to needing to create a move for the last move, which we don't get as a standalone json but with the whole game thing.
        /// </summary>
        public static MoveData ConstructFromFen(string infoFen)
        {
            MovePacket packet = new MovePacket()
            {
                fen = infoFen,
            };
            return new MoveData()
            {
                _data =  packet,
            };

        }
    }

    [System.Serializable]
    public struct MovePacket
    {
        public string fen;
        public string lm;
        public int wc;
        public int bc;
    }
}
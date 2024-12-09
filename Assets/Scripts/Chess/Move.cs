using UnityEngine;

namespace Chess
{
    [System.Serializable]
    public class Move
    {
        public string FEN => _data.fen;
        public string LastMove => _data.lm;
        private MoveData _data;
        public Move(string rawJSON)
        {
            //Move: {"fen":"r6r/1p3pkp/pQ3np1/3qNp2/3Pn3/7P/PP2NPP1/R2R2K1 b - - 0 20","lm":"e3d4","wc":126,"bc":127}
            _data = JsonUtility.FromJson<MoveData>(rawJSON);
            //Set board to FEN state minus the Last Move.
            //Animate the move. Move done and animated.
        }

        public override string ToString()
        {
            return _data.lm.ToString();
        }
        
    }

    [System.Serializable]
    public struct MoveData
    {
        public string fen;
        public string lm;
        public int wc;
        public int bc;
    }
}
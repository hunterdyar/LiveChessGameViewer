using System;
using Chess;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIPlayerBoxUpdater : MonoBehaviour
    {
        public PieceColor color;
        public TMP_Text playerNameText;
        public TMP_Text playerRatingText;
        private void OnEnable()
        {
            ChessLiveViewManager.OnNewGameInfo += OnNewInfo;
        }

        private void OnDisable()
        {
            ChessLiveViewManager.OnNewGameInfo -= OnNewInfo;
        }

        private void OnNewInfo(Info info)
        {
            if (color == PieceColor.White)
            {
                playerNameText.text = info.players.white.user.name;
                playerRatingText.text = info.players.white.rating.ToString();
            }else if(color == PieceColor.Black){
                playerNameText.text = info.players.black.user.name;
                playerRatingText.text = info.players.black.rating.ToString();
            }
            else
            {
                playerNameText.text = "";
            }
        }
    }
}
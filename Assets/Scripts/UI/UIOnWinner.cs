using System;
using Chess;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIOnWinner : MonoBehaviour
{ 
    public Image winnerPanel;
    public TMP_Text winnerText;
    
    private void Awake()
    {
        winnerPanel.gameObject.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        ChessGame.OnNewGameStart += OnNewGameStart;
        ChessGame.OnWinner += OnWinner;
    }
    private void OnDisable()
    {
        ChessGame.OnNewGameStart -= OnNewGameStart;
        ChessGame.OnWinner -= OnWinner;   
    }

    private void OnWinner(PieceColor winner)
    {
        winnerPanel.gameObject.SetActive(true);
        switch (winner)
        {
            case PieceColor.Black:
                winnerPanel.color = Color.black;
                winnerText.color = Color.white;
                winnerText.text = "Black Wins!";
                break;
            case PieceColor.White:
                winnerPanel.color = Color.white;
                winnerText.color = Color.black;
                winnerText.text = "White Wins!";
                break;
            default:
                //todo: uh oh, i forgot about draws.
                winnerPanel.color = Color.gray;
                winnerText.color  = Color.white;
                winnerText.text = "Draw!";
            return;
        }
    }

    private void OnNewGameStart()
    {
        winnerPanel.gameObject.SetActive(false);   
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}

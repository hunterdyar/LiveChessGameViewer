using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Chess;
using UnityEngine;
using UnityEngine.Networking;

public class ChessLiveViewManager : MonoBehaviour
{
    public static Action<GameState> OnGameStateChange;
    public static Action<Info> OnNewGameInfo;
    public GameState State => _state;
    private GameState _state;
    private string[] _availableChannels;
    public GameBoard Board;
    public string defaultGameType = "best";
    //
    public ChannelList _channelList;
    public Info CurrentInfo;
    public bool newGame = false;
    private Queue<Move> _pendingMoves = new Queue<Move>();
    private float gameOverTimer  = 0.0f;
    public float GameOverTime = 5f;
    private void Awake()
    {
        gameOverTimer = GameOverTime;
        Board = new GameBoard();
        ChangeState(GameState.SearchingForLiveGame,true);
    }

    void Start()
    { 
        FindNewGame();
    }

    private void Update()
    {
        if (_state == GameState.WatchingGame)
        {
            if (newGame)
            {
                //dispatch from main thread
                Board.CreateNewGame(CurrentInfo.fen);
                OnNewGameInfo?.Invoke(CurrentInfo);
                newGame = false;
            }

            if (_pendingMoves.Count > 0)
            {
                var m = _pendingMoves.Dequeue();
                Board.Move(m);
            }
        }else if (State == GameState.SearchingForLiveGame)
        {
            FindNewGame();
        }else if (State == GameState.GameComplete)
        {
            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0.0f)
            {
                ChangeState(GameState.SearchingForLiveGame);
                gameOverTimer = GameOverTime;
            }
        }
    }

    async void FindNewGame()
    {
        ChangeState(GameState.WatchingGame);
        await UpdateChannels();
        var game = _channelList.GetGameIDForChannelName(defaultGameType);
        HookIntoGame(game);
    }
    
    private void ChangeState(GameState state, bool sendUpdateIfSame = false)
    {
        if (state == _state && !sendUpdateIfSame)
        {
            return;
        }   
        
        _state = state;
        OnGameStateChange?.Invoke(_state);
    }

    private async Task UpdateChannels()
    {
        //https://lichess.org/api/tv/channels
        var request = UnityWebRequest.Get("https://lichess.org/api/tv/channels");
        await request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            var data = request.downloadHandler.text;
            _channelList = new ChannelList(data);
        }
    }
    
    void HookIntoGame(string gameID)
    {
        WebRequest request = WebRequest.Create($"https://lichess.org/api/stream/game/{gameID}");
        // request.Credentials = new NetworkCredential("username", "password");
        request.BeginGetResponse(ar => 
        {
            var req = (WebRequest)ar.AsyncState;
            // TODO: Add exception handling: EndGetResponse could throw
            using (var response = req.EndGetResponse(ar))
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                // This loop goes as long as twitter is streaming
                while (!reader.EndOfStream)
                {
                    CurrentGameUpdate(reader.ReadLine());
                }

                CurrentGameStreamEnded();
            }
        }, request);
        
    }

    private void CurrentGameStreamEnded()
    {
        Debug.Log("GAME ENDED");
        ChangeState(GameState.GameComplete);
    }

    void CurrentGameUpdate(string update)
    {
        if (update.StartsWith("{\"id"))
        {
            //this is the game start or the game finish.
            Info info = JsonUtility.FromJson<Info>(update);
            CurrentInfo = info;
            newGame = true;
        }
        else
        {
            //this is a single move
            Move m = new Move(update);
            _pendingMoves.Enqueue(m);
        }
    }

   
}

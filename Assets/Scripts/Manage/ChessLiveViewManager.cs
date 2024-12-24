using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Chess;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ChessLiveViewManager : MonoBehaviour
{
    public static Action<GameState> OnGameStateChange;
    public static Action<Info> OnNewGameInfo;
    public static Action OnShouldClose;
    public GameState State => _state;
    private GameState _state;
    private string[] _availableChannels;
    //
    public static ChannelList ChannelList => _channelList;
    public static ChannelList _channelList;
    public Info CurrentInfo;
    public bool newGame = false;
    private Queue<MoveData> _pendingMoves = new Queue<MoveData>();
    private float gameOverTimer  = 0.0f;
    public float GameOverTime = 2.5f;

    private float errorRetryCountdown = 3f;

    private UnityWebRequest _request;
    //refactor
    private ChessGame _game;

    public static ChessLiveViewManager Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Instance already exists");
        }
        Instance = this;
        
        _game = new ChessGame();
        gameOverTimer = GameOverTime;
        ChangeState(GameState.SearchingForLiveGame,true);
    }

    void Start()
    {
        GameSetings.LoadSetings();
        SceneManager.LoadSceneAsync("Settings", LoadSceneMode.Additive);
        _ = FindNewGame();
    }

    private void Update()
    {
        if (errorRetryCountdown <= 0)
        {
            errorRetryCountdown = 3f;
            ResetEverything();
        }
        if (_state == GameState.WatchingGame)
        {
            if (newGame)
            {
                //dispatch from main thread
                _game.Init(CurrentInfo);
                OnNewGameInfo?.Invoke(CurrentInfo);
                newGame = false;
            }

            if (_pendingMoves.Count > 0)
            {
                var m = _pendingMoves.Dequeue();
                _game.NextMove(m);
            }

            //this is the queue thing again, but animations will ask to block it.
            _game.Tick();
            //
            
        }else if (State == GameState.SearchingForLiveGame)
        {
            _=FindNewGame();
        }else if (State == GameState.GameComplete)
        {
            if (!_game.DoneDisplaying())
            {
                return;
            }
            gameOverTimer -= Time.deltaTime;
            if (gameOverTimer <= 0.0f)
            {
                ChangeState(GameState.SearchingForLiveGame);
                gameOverTimer = GameOverTime;
            }
        }else if (State == GameState.Error)
        {
            errorRetryCountdown -= Time.deltaTime;
        }
    }

    public void AskForNewGame()
    {
        _ = FindNewGame();
    }
    async Task FindNewGame()
    {
        ChangeState(GameState.WatchingGame);
        await UpdateChannels();
        var game = _channelList.GetGameIDForChannelName(GameSetings.ChessChannel);
        
        if (game == "")
        {
            ChangeState(GameState.Error);   
        }
        else
        {
            HookIntoGame(game);
        }
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
        _request = UnityWebRequest.Get("https://lichess.org/api/tv/channels");
        await _request.SendWebRequest();
        if (_request.result == UnityWebRequest.Result.Success)
        {
            var data = _request.downloadHandler.text;
            _channelList = new ChannelList(data);
        }
    }
    
    void HookIntoGame(string gameID)
    {
        if (_request != null)
        {
            //abort?
            _request.Dispose();
        }
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
            if (!string.IsNullOrEmpty(info.winner))
            {
                var winner = ChessGame.StringToColor(info.winner);
                var lastMove = MoveData.ConstructFromFen(info.fen);
                _pendingMoves.Enqueue(lastMove);
                _game.SetWinner(winner);
            }
            newGame = true;
        }
        else
        {
            //this is a single move
            MoveData m = new MoveData(update);
            _pendingMoves.Enqueue(m);
        }
    }


    public void AskForGameInit()
    {
        if (_state == GameState.WatchingGame)
        {
            OnNewGameInfo?.Invoke(CurrentInfo);
        }

        _game.RecreateRealPieces();
    }

    public static void ResetEverything()
    {
        OnShouldClose?.Invoke();
        //unload all other scenes.
        for (int i = 1; i < SceneManager.loadedSceneCount; i++)
        {
            var ulo = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).name);
        }

        //unload scene 0 and reload it. that's this scene!
        SceneManager.LoadScene(0);
    }
}

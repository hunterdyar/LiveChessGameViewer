using UnityEngine;
using UnityEngine.UI;

public class UIResetButton : MonoBehaviour
{

    private Button _button;

    private ChessLiveViewManager _chessLiveViewManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(Click);
    }

    void Click()
    {
        ChessLiveViewManager.ResetEverything();
    }
}
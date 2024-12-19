using UnityEngine;

public class SettingsPanelManager : MonoBehaviour
{
    public GameObject SettingsPanel;

    [SerializeField] float _doubleTapTimer = 0.5f;
    private float _tapTime = 0;
    void Start()
    {
        ClosePanel();
    }
    
    public void OpenPanel()
    {
     SettingsPanel.SetActive(true);   
    }

    public void ClosePanel()
    {
        SettingsPanel.SetActive(false);
    }

    public void TogglePanel()
    {
        SettingsPanel.SetActive(!SettingsPanel.activeInHierarchy);
    }

    void Update()
    {
        if (!SettingsPanel.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            if (Time.time - _tapTime < _doubleTapTimer)
            {
                TogglePanel();
            }
            _tapTime = Time.time;
        }
    }
}

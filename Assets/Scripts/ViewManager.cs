using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    private int _currentViewScene;
    public ViewScene[] Views; 
    IEnumerator Start()
    {
        _currentViewScene = GameSetings.ViewScene;
        var load = SceneManager.LoadSceneAsync(Views[_currentViewScene].SceneName, LoadSceneMode.Additive);

        while (!load.isDone)
        {
            //update progress bar
            var progress = load.progress;
            yield return null;
        }
    }

    public void SwitchView(int newView)
    {
        var name = Views[newView].SceneName;
        StartCoroutine(DoSwitchView(name));
    }
    public IEnumerator DoSwitchView(string sceneName)
    {
        var unload = SceneManager.UnloadSceneAsync(Views[_currentViewScene].SceneName);
        var load= SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!(unload.isDone && load.isDone))
        {
            yield return null;
        }
        //done!
    }

    [Serializable]
    public struct ViewScene
    {
        public string DisplayName;
        public string SceneName;
    }
}

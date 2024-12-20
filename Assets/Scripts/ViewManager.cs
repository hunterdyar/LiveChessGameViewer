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
        GameSetings.LoadSetings();//do it twice because I am too lazy to fix the race condition and call this function from the manager s
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
        StartCoroutine(DoSwitchView(newView));
    }
    public IEnumerator DoSwitchView(int newIndex)
    {
        var sceneName = Views[newIndex].SceneName;

        var loaded = Views[_currentViewScene].SceneName;
        var s = SceneManager.GetSceneByName(loaded);
        if (!s.isLoaded)
        {
            Debug.LogWarning("Scenes out of sync...");
        }
        var unload = SceneManager.UnloadSceneAsync(s);
        var load= SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!(unload.isDone && load.isDone))
        {
            yield return null;
        }

        _currentViewScene = newIndex;
        GameSetings.SetViewScene(_currentViewScene);
        //done!
    }

    [Serializable]
    public struct ViewScene
    {
        public string DisplayName;
        public string SceneName;
    }
}

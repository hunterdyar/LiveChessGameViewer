using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    public string displayScene;

    private string currentExtraScene;
    
    IEnumerator Start()
    {
        currentExtraScene = displayScene;
        
        var load = SceneManager.LoadSceneAsync(displayScene, LoadSceneMode.Additive);

        while (!load.isDone)
        {
            //update progress bar
            var progress = load.progress;
            yield return null;
        }
    }

    public IEnumerator SwitchView()
    {
        var unload = SceneManager.UnloadSceneAsync(currentExtraScene);
        var load= SceneManager.LoadSceneAsync(displayScene, LoadSceneMode.Additive);
        yield break;
    }
}

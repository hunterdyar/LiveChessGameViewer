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
        
        //done
        SceneManager.MoveGameObjectToScene();
    }

    public void SwitchView()
    {
        SceneManager.UnloadScene(currentExtraScene);
    }
}

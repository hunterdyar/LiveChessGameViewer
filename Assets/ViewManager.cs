using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    public string displayScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.LoadScene(displayScene, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

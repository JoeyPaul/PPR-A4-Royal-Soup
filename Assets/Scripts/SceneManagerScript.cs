using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public static SceneManagerScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }

    public static void ChangeScene(string receivedName)
    {
        if (SceneManager.GetSceneByName(receivedName) != null)
            instance.StartCoroutine(instance.TransitionToScene(receivedName));
        else
            print("Could not find Scene of name: " + receivedName);
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(sceneName);
    }

    public static void Quit()
    {
        Application.Quit();
    }
}

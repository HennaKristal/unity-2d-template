using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;
    private Fading fading;
    private Coroutine sceneRoutine;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        fading = GetComponent<Fading>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }


    public void ExitApplication()
    {
        Application.Quit();
    }

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fading != null)
        {
            fading.StartFadeIn(2f);
        }
    }

    public void LoadSceneByName(string sceneName)
    {
        if (sceneRoutine != null)
        {
            StopCoroutine(sceneRoutine);
        }

        sceneRoutine = StartCoroutine(ChangeScene(sceneName));
    }

    private IEnumerator ChangeScene(string sceneName)
    {
        if (fading != null)
        {
            fading.StartFadeOut(2f);
        }

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneName);
    }
}

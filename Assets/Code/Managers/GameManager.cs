using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private Fading fading;
    private Coroutine sceneRoutine;

    [Header("REFERENCES")]
    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();

        fading = GetComponent<Fading>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

    // =====================================================
    // References
    // =====================================================

    public void SetPlayerTransform(Transform transform)
    {
        playerTransform = transform;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }
}

using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : Singleton<GameManager>
{
    private Fading fading;
    private Coroutine sceneRoutine;
    private Transform playerTransform;
    public Action<Transform> OnPlayerTransformChanged;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player != null ? player.transform : null;
        OnPlayerTransformChanged?.Invoke(playerTransform);

        if (fading != null)
        {
            fading.StartFadeIn(2.0f);
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
            fading.StartFadeOut(2.0f);
        }

        yield return new WaitForSeconds(2.0f);
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

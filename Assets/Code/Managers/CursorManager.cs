using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class CursorAnimation
{
    public CursorManager.CursorType cursorType;
    public Texture2D[] textureArray;
    public float animationFrameTime;
    public Vector2 offset;
}

public class CursorManager : Singleton<CursorManager>
{
    [SerializeField] private List<CursorAnimation> cursorAnimationList;
    [SerializeField] private CursorType defaultCursorType;
    private Dictionary<CursorType, CursorAnimation> cursorLookup;
    private CursorAnimation activeAnimation;
    private int activeFrameIndex;
    private int frameCount;
    private float frameTimer;
    private bool isCursorLocked;
    private float cursorLockTimer;
    private CursorType? pendingCursorType;
    private CursorType currentCursorType;

    public enum CursorType
    {
        Pointer,
        Combat,
        Wait
    }

    protected override void Awake()
    {
        base.Awake();
        BuildCursorLookup();
    }

    private void Start()
    {
        SetDefaultCursor();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetDefaultCursor();
    }

    private void Update()
    {
        UpdateCursorLock();
        UpdateCursorAnimation();
    }

    private void BuildCursorLookup()
    {
        cursorLookup = new Dictionary<CursorType, CursorAnimation>();

        foreach (CursorAnimation animation in cursorAnimationList)
        {
            if (animation == null)
                continue;

            if (animation.textureArray == null || animation.textureArray.Length == 0)
                continue;

            cursorLookup[animation.cursorType] = animation;
        }
    }

    private void UpdateCursorLock()
    {
        if (!isCursorLocked)
            return;

        cursorLockTimer -= Time.deltaTime;

        if (cursorLockTimer > 0f)
            return;

        isCursorLocked = false;

        if (pendingCursorType.HasValue)
        {
            ApplyCursorAnimation(GetCursorAnimation(pendingCursorType.Value));
            pendingCursorType = null;
        }
    }

    private void UpdateCursorAnimation()
    {
        if (activeAnimation == null)
            return;

        if (activeAnimation.textureArray == null || activeAnimation.textureArray.Length == 0)
            return;

        if (activeAnimation.animationFrameTime <= 0f)
            return;

        frameTimer -= Time.deltaTime;

        if (frameTimer > 0f)
            return;

        frameTimer += activeAnimation.animationFrameTime;
        activeFrameIndex = (activeFrameIndex + 1) % frameCount;

        Cursor.SetCursor(
            activeAnimation.textureArray[activeFrameIndex],
            activeAnimation.offset,
            CursorMode.Auto
        );
    }

    public void SetDefaultCursor()
    {
        SetActiveCursorType(defaultCursorType);
    }

    public void SetActiveCursorType(CursorType cursorType)
    {
        if (isCursorLocked)
        {
            pendingCursorType = cursorType;
            return;
        }

        pendingCursorType = null;
        ApplyCursorAnimation(GetCursorAnimation(cursorType));
    }

    private CursorAnimation GetCursorAnimation(CursorType cursorType)
    {
        if (cursorLookup.TryGetValue(cursorType, out CursorAnimation animation))
            return animation;

        return null;
    }

    private void ApplyCursorAnimation(CursorAnimation animation)
    {
        if (animation == null || animation.textureArray == null || animation.textureArray.Length == 0)
        {
            activeAnimation = null;
            currentCursorType = defaultCursorType;

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        activeAnimation = animation;
        activeFrameIndex = 0;
        frameTimer = animation.animationFrameTime;
        frameCount = animation.textureArray.Length;
        currentCursorType = animation.cursorType;

        Cursor.SetCursor(
            animation.textureArray[0],
            animation.offset,
            CursorMode.Auto
        );
    }

    public void LockCursorType(float duration)
    {
        isCursorLocked = true;
        cursorLockTimer = duration;
    }

    public void UnlockCursorType()
    {
        isCursorLocked = false;

        if (pendingCursorType.HasValue)
        {
            ApplyCursorAnimation(GetCursorAnimation(pendingCursorType.Value));
            pendingCursorType = null;
        }
    }

    public CursorType GetCurrentCursorType()
    {
        return currentCursorType;
    }
}

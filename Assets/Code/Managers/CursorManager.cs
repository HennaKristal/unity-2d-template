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
    private CursorAnimation cursorAnimation;
    private int currentCursorFrame;
    private int cursorFrameCount;
    private float cursorFrameTimer;
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

        // BuildCursorLookup
        cursorLookup = new Dictionary<CursorType, CursorAnimation>();
        foreach (CursorAnimation animation in cursorAnimationList)
        {
            if (animation != null && animation.textureArray != null && animation.textureArray.Length > 0)
            {
                cursorLookup[animation.cursorType] = animation;
            }
        }
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

    public void SetDefaultCursor()
    {
        SetActiveCursorType(defaultCursorType);
    }

    private void Update()
    {
        UpdateCursorLock();
        UpdateCursorAnimation();
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
            SetActiveCursorAnimation(GetCursorAnimation(pendingCursorType.Value));
            pendingCursorType = null;
        }
    }

    private void UpdateCursorAnimation()
    {
        if (cursorAnimation == null || cursorAnimation.textureArray == null)
            return;

        if (cursorAnimation.textureArray.Length == 0 || cursorAnimation.animationFrameTime <= 0f)
            return;

        cursorFrameTimer -= Time.deltaTime;

        if (cursorFrameTimer > 0f)
            return;

        cursorFrameTimer += cursorAnimation.animationFrameTime;
        currentCursorFrame = (currentCursorFrame + 1) % cursorFrameCount;

        Cursor.SetCursor(
            cursorAnimation.textureArray[currentCursorFrame],
            cursorAnimation.offset,
            CursorMode.Auto
        );
    }

    public void SetActiveCursorType(CursorType cursorType)
    {
        if (isCursorLocked)
        {
            pendingCursorType = cursorType;
            return;
        }

        pendingCursorType = null;
        SetActiveCursorAnimation(GetCursorAnimation(cursorType));
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
            SetActiveCursorAnimation(GetCursorAnimation(pendingCursorType.Value));
            pendingCursorType = null;
        }
    }

    private CursorAnimation GetCursorAnimation(CursorType cursorType)
    {
        if (cursorLookup.TryGetValue(cursorType, out CursorAnimation animation))
        {
            return animation;
        }

        return null;
    }

    private void SetActiveCursorAnimation(CursorAnimation newAnimation)
    {
        if (newAnimation == null || newAnimation.textureArray == null || newAnimation.textureArray.Length == 0)
        {
            cursorAnimation = null;
            currentCursorType = defaultCursorType;

            Cursor.SetCursor(
                null,
                Vector2.zero,
                CursorMode.Auto
            );

            return;
        }

        cursorAnimation = newAnimation;
        currentCursorFrame = 0;
        cursorFrameTimer = cursorAnimation.animationFrameTime;
        cursorFrameCount = cursorAnimation.textureArray.Length;
        currentCursorType = newAnimation.cursorType;

        Cursor.SetCursor(
            cursorAnimation.textureArray[currentCursorFrame],
            cursorAnimation.offset,
            CursorMode.Auto
        );
    }

    public CursorType GetCurrentCursorType()
    {
        return currentCursorType;
    }

}

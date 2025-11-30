using System.Collections.Generic;
using UnityEngine;
using System;

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
        SetDefaultCursorType();
    }

    private void Update()
    {
        if (cursorAnimation == null || cursorAnimation.textureArray == null)
            return;

        if (cursorAnimation.textureArray.Length == 0 || cursorAnimation.animationFrameTime <= 0f)
            return;

        cursorFrameTimer -= Time.deltaTime;

        if (cursorFrameTimer <= 0f)
        {
            cursorFrameTimer += cursorAnimation.animationFrameTime;
            currentCursorFrame = (currentCursorFrame + 1) % cursorFrameCount;

            Cursor.SetCursor(
                cursorAnimation.textureArray[currentCursorFrame],
                cursorAnimation.offset,
                CursorMode.Auto
            );
        }
    }

    public void SetActiveCursorType(CursorType cursorType)
    {
        SetActiveCursorAnimation(GetCursorAnimation(cursorType));
    }

    public void SetDefaultCursorType()
    {
        SetActiveCursorAnimation(GetCursorAnimation(defaultCursorType));
    }

    private void BuildCursorLookup()
    {
        cursorLookup = new Dictionary<CursorType, CursorAnimation>();

        foreach (CursorAnimation animation in cursorAnimationList)
        {
            if (animation != null && animation.textureArray != null && animation.textureArray.Length > 0)
            {
                cursorLookup[animation.cursorType] = animation;
            }
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
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        cursorAnimation = newAnimation;
        currentCursorFrame = 0;
        cursorFrameTimer = cursorAnimation.animationFrameTime;
        cursorFrameCount = cursorAnimation.textureArray.Length;

        Cursor.SetCursor(
            cursorAnimation.textureArray[currentCursorFrame],
            cursorAnimation.offset,
            CursorMode.Auto
        );
    }
}

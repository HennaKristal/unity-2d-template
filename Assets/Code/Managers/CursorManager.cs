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


public class CursorManager : MonoBehaviour
{
    private static CursorManager _instance;
    public static CursorManager Instance => _instance;

    [SerializeField] private List<CursorAnimation> cursorAnimationList;
    [SerializeField] private CursorType defaultCursorType;

    private CursorAnimation cursorAnimation;
    private int currentCursorFrame;
    private int cursorFrameCount;
    private float cursorFrameTimer;

    public enum CursorType
    {
        Pointer,
        Combat,
        Talk
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetDefaultCursorType();
    }

    private void Update()
    {
        if (cursorAnimation != null && cursorAnimation.textureArray != null && cursorAnimation.textureArray.Length > 0 && cursorAnimation.animationFrameTime > 0f)
        {
            cursorFrameTimer -= Time.deltaTime;
            if (cursorFrameTimer <= 0f)
            {
                cursorFrameTimer += cursorAnimation.animationFrameTime;
                currentCursorFrame = (currentCursorFrame + 1) % cursorFrameCount;
                Cursor.SetCursor(cursorAnimation.textureArray[currentCursorFrame], cursorAnimation.offset, CursorMode.Auto);
            }
        }
    }

    public void SetAciveCursorType(CursorType cursorType)
    {
        SetActiveCursorAnimation(GetCursorAnimation(cursorType));
    }

    public void SetDefaultCursorType()
    {
        SetActiveCursorAnimation(GetCursorAnimation(defaultCursorType));
    }

    private CursorAnimation GetCursorAnimation(CursorType cursorType)
    {
        foreach (CursorAnimation anim in cursorAnimationList)
        {
            if (anim != null && anim.cursorType == cursorType)
            {
                return anim;
            }
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
        Cursor.SetCursor(cursorAnimation.textureArray[currentCursorFrame], cursorAnimation.offset, CursorMode.Auto);
    }
}
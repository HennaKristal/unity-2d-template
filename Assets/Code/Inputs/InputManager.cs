using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private const string RemappedHotkeysKey = "remapped-hotkeys";

    private PlayerInputActions playerInputActions;
    private PlayerInputActions.GameplayActions gameplayInputs;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public Vector2 Move { get; private set; }

    public bool JumpPress { get; private set; }
    public bool JumpRelease { get; private set; }
    public bool JumpHold { get; private set; }

    public bool ScreenshotPress { get; private set; }

    public bool UIEnterPress { get; private set; }
    public bool UIEnterRelease { get; private set; }
    public bool UIEnterHold { get; private set; }

    public bool UICancelPress { get; private set; }
    public bool UICancelRelease { get; private set; }
    public bool UICancelHold { get; private set; }

    public bool IsRebinding { get; private set; }

    public InputActionAsset InputActions => playerInputActions.asset;

    protected override void Awake()
    {
        base.Awake();

        playerInputActions = new PlayerInputActions();
        gameplayInputs = playerInputActions.Gameplay;

        LoadBindingOverrides();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        CancelRebind();

        if (Instance == this && playerInputActions != null)
        {
            playerInputActions.Disable();
        }
    }

    private void Update()
    {
        Move = gameplayInputs.Move.ReadValue<Vector2>();

        JumpPress = gameplayInputs.Jump.WasPressedThisFrame();
        JumpRelease = gameplayInputs.Jump.WasReleasedThisFrame();
        JumpHold = gameplayInputs.Jump.IsPressed();

        ScreenshotPress = gameplayInputs.Screenshot.WasPressedThisFrame();

        UIEnterPress = gameplayInputs.UIEnter.WasPressedThisFrame();
        UIEnterRelease = gameplayInputs.UIEnter.WasReleasedThisFrame();
        UIEnterHold = gameplayInputs.UIEnter.IsPressed();

        UICancelPress = gameplayInputs.UICancel.WasPressedThisFrame();
        UICancelRelease = gameplayInputs.UICancel.WasReleasedThisFrame();
        UICancelHold = gameplayInputs.UICancel.IsPressed();
    }

    public InputAction GetAction(string actionName)
    {
        return playerInputActions.asset.FindAction(actionName, true);
    }

    public string GetBindingDisplayString(string actionName, int bindingIndex)
    {
        InputAction action = GetAction(actionName);

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            return string.Empty;
        }

        return action.GetBindingDisplayString(bindingIndex);
    }

    public void StartRebind(
        string actionName,
        int bindingIndex,
        string allowedControlPath,
        float timeout,
        System.Action onComplete,
        System.Action onCancel)
    {
        CancelRebind();

        InputAction action = GetAction(actionName);

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            return;
        }

        IsRebinding = true;

        action.Disable();

        rebindingOperation = action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsHavingToMatchPath(allowedControlPath)
            .WithTimeout(timeout)
            .OnCancel(operation =>
            {
                FinishRebind(action);
                onCancel?.Invoke();
            })
            .OnComplete(operation =>
            {
                FinishRebind(action);
                SaveBindingOverrides();
                onComplete?.Invoke();
            });

        rebindingOperation.Start();
    }

    public void CancelRebind()
    {
        if (rebindingOperation == null)
        {
            return;
        }

        rebindingOperation.Cancel();
    }

    public void ResetBinding(string actionName, int bindingIndex)
    {
        InputAction action = GetAction(actionName);

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            return;
        }

        action.RemoveBindingOverride(bindingIndex);
        SaveBindingOverrides();
    }

    public void ResetAllBindings()
    {
        playerInputActions.asset.RemoveAllBindingOverrides();
        SaveBindingOverrides();
    }

    private void FinishRebind(InputAction action)
    {
        rebindingOperation?.Dispose();
        rebindingOperation = null;

        action.Enable();
        IsRebinding = false;
    }

    private void SaveBindingOverrides()
    {
        string overrides = playerInputActions.asset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RemappedHotkeysKey, overrides);
        PlayerPrefs.Save();
    }

    public string GetBindingEffectivePath(string actionName, int bindingIndex)
    {
        InputAction action = GetAction(actionName);

        if (bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            return string.Empty;
        }

        return action.bindings[bindingIndex].effectivePath;
    }

    private void LoadBindingOverrides()
    {
        string overrides = PlayerPrefs.GetString(RemappedHotkeysKey, string.Empty);

        if (string.IsNullOrEmpty(overrides))
        {
            return;
        }

        playerInputActions.asset.LoadBindingOverridesFromJson(overrides);
    }
}
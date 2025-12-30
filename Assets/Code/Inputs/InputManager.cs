using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInputActions playerInputActions;
    private PlayerInputActions.GameplayActions gameplayInputs;

    public Vector2 Move { get; private set; }
    public bool screenshotPress { get; private set; }
    public bool UIEnterPress { get; private set; }
    public bool UIEnterRelease { get; private set; }
    public bool UIEnterHold { get; private set; }
    public bool UICancelPress { get; private set; }
    public bool UICancelRelease { get; private set; }
    public bool UICancelHold { get; private set; }

    protected override void Awake()
    {
        base.Awake();
  
        playerInputActions = new PlayerInputActions();

        string overrides = PlayerPrefs.GetString("remapped-hotkeys", "");
        if (!string.IsNullOrEmpty(overrides))
        {
            playerInputActions.asset.LoadBindingOverridesFromJson(overrides);
        }

        gameplayInputs = playerInputActions.Gameplay;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        if (Instance == this && playerInputActions != null)
        {
            playerInputActions.Disable();
        }
    }

    private void Update()
    {
        Move = gameplayInputs.Move.ReadValue<Vector2>();

        screenshotPress = gameplayInputs.Screenshot.WasPressedThisFrame();

        UIEnterPress = gameplayInputs.UIEnter.WasPressedThisFrame();
        UIEnterRelease = gameplayInputs.UIEnter.WasReleasedThisFrame();
        UIEnterHold = gameplayInputs.UIEnter.IsPressed();

        UICancelPress = gameplayInputs.UICancel.WasPressedThisFrame();
        UICancelRelease = gameplayInputs.UICancel.WasReleasedThisFrame();
        UICancelHold = gameplayInputs.UICancel.IsPressed();
    }
}

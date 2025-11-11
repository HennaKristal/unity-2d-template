using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private PlayerInputActions playerInputActions;

    public Vector2 Move { get; private set; }

    public bool ScreenshotPressed { get; private set; }
    public bool ScreenshotReleased { get; private set; }
    public bool ScreenshotHeld { get; private set; }

    public bool UIEnterPressed { get; private set; }
    public bool UIEnterReleased { get; private set; }
    public bool UIEnterHeld { get; private set; }

    public bool UICancelPressed { get; private set; }
    public bool UICancelReleased { get; private set; }
    public bool UICancelHeld { get; private set; }

    private PlayerInputActions PlayerInputActions
    {
        get
        {
            if (playerInputActions == null)
            {
                playerInputActions = new PlayerInputActions();
            }

            return playerInputActions;
        }
    }

    private void OnEnable()
    {
        PlayerInputActions.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.Disable();
    }

    private void Update()
    {
        Move = PlayerInputActions.Gameplay.Move.ReadValue<Vector2>();

        UIEnterPressed = PlayerInputActions.Gameplay.UIEnter.WasPressedThisFrame();
        UIEnterReleased = PlayerInputActions.Gameplay.UIEnter.WasReleasedThisFrame();
        UIEnterHeld = PlayerInputActions.Gameplay.UIEnter.IsPressed();

        ScreenshotPressed = PlayerInputActions.Gameplay.Screenshot.WasPressedThisFrame();
        ScreenshotReleased = PlayerInputActions.Gameplay.Screenshot.WasReleasedThisFrame();
        ScreenshotHeld = PlayerInputActions.Gameplay.Screenshot.IsPressed();

        UICancelPressed = PlayerInputActions.Gameplay.UICancel.WasPressedThisFrame();
        UICancelReleased = PlayerInputActions.Gameplay.UICancel.WasReleasedThisFrame();
        UICancelHeld = PlayerInputActions.Gameplay.UICancel.IsPressed();
    }
}

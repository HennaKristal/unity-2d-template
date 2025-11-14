using UnityEngine;

public class InputController : MonoBehaviour
{
    private static InputController _instance;
    public static InputController Instance => _instance;

    private PlayerInputActions playerInputActions;
    private PlayerInputActions.GameplayActions gameplayInputs;

    public Vector2 Move { get; private set; }
    public bool screenshotPress { get; private set; }
    public bool screenshotRelease { get; private set; }
    public bool screenshotHold { get; private set; }
    public bool UIEnterPress { get; private set; }
    public bool UIEnterRelease { get; private set; }
    public bool UIEnterHold { get; private set; }
    public bool UICancelPress { get; private set; }
    public bool UICancelRelease { get; private set; }
    public bool UICancelHold { get; private set; }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        playerInputActions = new PlayerInputActions();
        gameplayInputs = playerInputActions.Gameplay;
    }


    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }


    private void Update()
    {
        Move = gameplayInputs.Move.ReadValue<Vector2>();

        screenshotPress = gameplayInputs.Screenshot.WasPressedThisFrame();
        screenshotRelease = gameplayInputs.Screenshot.WasReleasedThisFrame();
        screenshotHold = gameplayInputs.Screenshot.IsPressed();

        UIEnterPress = gameplayInputs.UIEnter.WasPressedThisFrame();
        UIEnterRelease = gameplayInputs.UIEnter.WasReleasedThisFrame();
        UIEnterHold = gameplayInputs.UIEnter.IsPressed();

        UICancelPress = gameplayInputs.UICancel.WasPressedThisFrame();
        UICancelRelease = gameplayInputs.UICancel.WasReleasedThisFrame();
        UICancelHold = gameplayInputs.UICancel.IsPressed();
    }
}

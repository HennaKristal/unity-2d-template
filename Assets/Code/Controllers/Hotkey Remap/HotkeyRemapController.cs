using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotkeyRemapController : MonoBehaviour
{
    [System.Serializable]
    public class RemapRow
    {
        public RemapKeyCell keyboardCell;
        public RemapKeyCell controllerCell;
    }

    public List<RemapRow> rows = new List<RemapRow>();
    public Button returnButton;

    public float moveRepeatDelay = 0.2f;

    private int currentRowIndex;
    private int currentColumnIndex;
    private float navigationCooldownTimer;
    private bool panelActive;

    private void OnEnable()
    {
        panelActive = true;
        currentRowIndex = 0;
        currentColumnIndex = 0;
        navigationCooldownTimer = 0f;

        HookCellEvents();
        UpdateHighlight();
    }

    private void OnDisable()
    {
        panelActive = false;
    }

    private void Update()
    {
        if (!panelActive)
        {
            return;
        }

        navigationCooldownTimer -= Time.unscaledDeltaTime;

        HandleNavigation();
        HandleSubmit();
        HandleCancel();
    }

    private void HandleNavigation()
    {
        Vector2 moveVector = InputManager.Instance.Move;

        if (navigationCooldownTimer > 0f)
        {
            return;
        }

        bool moved = false;

        if (moveVector.y > 0.5f)
        {
            currentRowIndex--;
            moved = true;
        }
        else if (moveVector.y < -0.5f)
        {
            currentRowIndex++;
            moved = true;
        }
        else if (moveVector.x < -0.5f)
        {
            currentColumnIndex = 0;
            moved = true;
        }
        else if (moveVector.x > 0.5f)
        {
            currentColumnIndex = 1;
            moved = true;
        }

        if (moved)
        {
            ClampSelection();
            UpdateHighlight();
            navigationCooldownTimer = moveRepeatDelay;
        }
    }

    private void HandleSubmit()
    {
        if (!InputManager.Instance.UIEnterPress)
        {
            return;
        }

        RemapKeyCell cell = GetCurrentCell();
        if (cell != null)
        {
            cell.button.onClick.Invoke();
        }
    }

    private void HandleCancel()
    {
        if (!InputManager.Instance.UICancelPress)
        {
            return;
        }

        ClosePanel();
    }

    private void ClosePanel()
    {
        if (returnButton != null)
        {
            returnButton.onClick.Invoke();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ClampSelection()
    {
        if (rows.Count == 0)
        {
            currentRowIndex = 0;
            currentColumnIndex = 0;
            return;
        }

        if (currentRowIndex < 0)
        {
            currentRowIndex = 0;
        }
        if (currentRowIndex >= rows.Count)
        {
            currentRowIndex = rows.Count - 1;
        }

        if (currentColumnIndex < 0)
        {
            currentColumnIndex = 0;
        }
        if (currentColumnIndex > 1)
        {
            currentColumnIndex = 1;
        }
    }

    private RemapKeyCell GetCurrentCell()
    {
        if (rows.Count == 0)
        {
            return null;
        }

        RemapRow row = rows[currentRowIndex];
        return currentColumnIndex == 0 ? row.keyboardCell : row.controllerCell;
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            RemapRow row = rows[i];

            if (row.keyboardCell != null)
            {
                bool highlight = i == currentRowIndex && currentColumnIndex == 0;
                row.keyboardCell.SetHighlighted(highlight);
            }

            if (row.controllerCell != null)
            {
                bool highlight = i == currentRowIndex && currentColumnIndex == 1;
                row.controllerCell.SetHighlighted(highlight);
            }
        }
    }

    private void HookCellEvents()
    {
        for (int i = 0; i < rows.Count; i++)
        {
            int rowIndexCopy = i;

            RemapRow row = rows[i];

            if (row.keyboardCell != null)
            {
                row.keyboardCell.onPointerEntered = OnCellPointerEnter;
                row.keyboardCell.onPointerClicked = OnCellPointerClick;
            }

            if (row.controllerCell != null)
            {
                row.controllerCell.onPointerEntered = OnCellPointerEnter;
                row.controllerCell.onPointerClicked = OnCellPointerClick;
            }
        }
    }

    private void OnCellPointerEnter(RemapKeyCell cell)
    {
        for (int i = 0; i < rows.Count; i++)
        {
            RemapRow row = rows[i];

            if (row.keyboardCell == cell)
            {
                currentRowIndex = i;
                currentColumnIndex = 0;
                UpdateHighlight();
                return;
            }

            if (row.controllerCell == cell)
            {
                currentRowIndex = i;
                currentColumnIndex = 1;
                UpdateHighlight();
                return;
            }
        }
    }

    private void OnCellPointerClick(RemapKeyCell cell)
    {
        if (cell != null)
        {
            cell.button.onClick.Invoke();
        }
    }
}

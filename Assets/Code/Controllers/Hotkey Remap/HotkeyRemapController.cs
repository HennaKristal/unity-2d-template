using System.Collections.Generic;
using TMPro;
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

    [Header("Rows")]
    [SerializeField] private List<RemapRow> rows = new List<RemapRow>();

    [Header("Navigation")]
    [SerializeField] private float navigationRepeatDelay = 0.2f;

    [Header("Rebinding")]
    [SerializeField] private float rebindDuration = 5.0f;

    private int currentRowIndex;
    private int currentColumnIndex;

    private float navigationCooldownTimer;
    private float rebindTimer;

    private bool panelActive;
    private bool isRebinding;

    private RemapKeyCell activeRebindCell;

    private void OnEnable()
    {
        panelActive = true;

        currentRowIndex = 0;
        currentColumnIndex = 0;

        navigationCooldownTimer = 0.0f;

        HookCellEvents();

        RefreshAllCells();
        UpdateHighlight();
        CheckDuplicateBindings();
    }

    private void OnDisable()
    {
        panelActive = false;

        if (isRebinding)
        {
            CancelRebind();
        }
    }

    private void Update()
    {
        if (!panelActive)
        {
            return;
        }

        if (isRebinding)
        {
            UpdateRebindTimer();
            return;
        }

        navigationCooldownTimer -= Time.unscaledDeltaTime;

        HandleNavigation();
        HandleSubmit();
    }

    private void HandleNavigation()
    {
        if (navigationCooldownTimer > 0.0f)
        {
            return;
        }

        Vector2 movement = InputManager.Instance.Move;

        bool moved = false;

        if (movement.y > 0.5f)
        {
            currentRowIndex--;
            moved = true;
        }
        else if (movement.y < -0.5f)
        {
            currentRowIndex++;
            moved = true;
        }
        else if (movement.x < -0.5f)
        {
            currentColumnIndex = 0;
            moved = true;
        }
        else if (movement.x > 0.5f)
        {
            currentColumnIndex = 1;
            moved = true;
        }

        if (!moved)
        {
            return;
        }

        ClampSelection();
        UpdateHighlight();

        navigationCooldownTimer = navigationRepeatDelay;
    }

    private void HandleSubmit()
    {
        if (!InputManager.Instance.UIEnterPress)
        {
            return;
        }

        RemapKeyCell cell = GetCurrentCell();

        if (cell == null)
        {
            return;
        }

        StartRebind(cell);
    }

    private void StartRebind(RemapKeyCell cell)
    {
        if (cell == null || !cell.CanRebind)
        {
            return;
        }

        if (isRebinding)
        {
            return;
        }

        string allowedControlPath = cell.BindingInputType ==
            RemapKeyCell.InputType.Keyboard
            ? "<Keyboard>"
            : "<Gamepad>";

        activeRebindCell = cell;
        isRebinding = true;
        rebindTimer = rebindDuration;

        activeRebindCell.StartListening();

        InputManager.Instance.StartRebind(
            cell.ActionName,
            cell.BindingIndex,
            allowedControlPath,
            rebindDuration,
            CompleteRebind,
            CancelRebindVisuals
        );
    }

    private void UpdateRebindTimer()
    {
        if (activeRebindCell == null)
        {
            return;
        }

        rebindTimer -= Time.unscaledDeltaTime;

        float progress = rebindTimer / rebindDuration;

        activeRebindCell.SetListeningProgress(progress);
    }

    private void CompleteRebind()
    {
        if (activeRebindCell != null)
        {
            activeRebindCell.StopListening();
        }

        activeRebindCell = null;
        isRebinding = false;

        RefreshAllCells();
        CheckDuplicateBindings();
    }

    private void CancelRebind()
    {
        InputManager.Instance.CancelRebind();
        CancelRebindVisuals();
    }

    private void CancelRebindVisuals()
    {
        if (activeRebindCell != null)
        {
            activeRebindCell.StopListening();
        }

        activeRebindCell = null;
        isRebinding = false;

        RefreshAllCells();
        CheckDuplicateBindings();
    }

    public void ResetAllKeys()
    {
        if (isRebinding)
        {
            CancelRebind();
        }

        InputManager.Instance.ResetAllBindings();

        RefreshAllCells();
        CheckDuplicateBindings();
    }

    private void RefreshAllCells()
    {
        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            RemapRow row = rows[rowIndex];

            if (row.keyboardCell != null)
            {
                row.keyboardCell.RefreshBindingText();
            }

            if (row.controllerCell != null)
            {
                row.controllerCell.RefreshBindingText();
            }
        }
    }

    private void CheckDuplicateBindings()
    {
        List<RemapKeyCell> cells = GetAllCells();

        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
        {
            cells[cellIndex].SetDuplicateWarning(false);
        }

        for (int firstIndex = 0; firstIndex < cells.Count; firstIndex++)
        {
            RemapKeyCell firstCell = cells[firstIndex];
            string firstPath = firstCell.GetEffectivePath();

            if (string.IsNullOrEmpty(firstPath))
            {
                continue;
            }

            for (
                int secondIndex = firstIndex + 1;
                secondIndex < cells.Count;
                secondIndex++
            )
            {
                RemapKeyCell secondCell = cells[secondIndex];

                if (firstCell.BindingInputType != secondCell.BindingInputType)
                {
                    continue;
                }

                string secondPath = secondCell.GetEffectivePath();

                if (string.IsNullOrEmpty(secondPath))
                {
                    continue;
                }

                if (firstPath != secondPath)
                {
                    continue;
                }

                firstCell.SetDuplicateWarning(true);
                secondCell.SetDuplicateWarning(true);
            }
        }
    }

    private List<RemapKeyCell> GetAllCells()
    {
        List<RemapKeyCell> cells = new List<RemapKeyCell>();

        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            RemapRow row = rows[rowIndex];

            if (row.keyboardCell != null)
            {
                cells.Add(row.keyboardCell);
            }

            if (row.controllerCell != null)
            {
                cells.Add(row.controllerCell);
            }
        }

        return cells;
    }

    private void HookCellEvents()
    {
        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            RemapRow row = rows[rowIndex];

            if (row.keyboardCell != null)
            {
                row.keyboardCell.PointerEntered = OnCellPointerEnter;
                row.keyboardCell.PointerClicked = OnCellPointerClick;
            }

            if (row.controllerCell != null)
            {
                row.controllerCell.PointerEntered = OnCellPointerEnter;
                row.controllerCell.PointerClicked = OnCellPointerClick;
            }
        }
    }

    private void OnCellPointerEnter(RemapKeyCell cell)
    {
        if (isRebinding)
        {
            return;
        }

        FindCellPosition(
            cell,
            out currentRowIndex,
            out currentColumnIndex
        );

        UpdateHighlight();
    }

    private void OnCellPointerClick(RemapKeyCell cell)
    {
        if (isRebinding)
        {
            return;
        }

        FindCellPosition(
            cell,
            out currentRowIndex,
            out currentColumnIndex
        );

        UpdateHighlight();
        StartRebind(cell);
    }

    private void FindCellPosition(
        RemapKeyCell targetCell,
        out int rowIndex,
        out int columnIndex)
    {
        rowIndex = 0;
        columnIndex = 0;

        for (int index = 0; index < rows.Count; index++)
        {
            RemapRow row = rows[index];

            if (row.keyboardCell == targetCell)
            {
                rowIndex = index;
                columnIndex = 0;
                return;
            }

            if (row.controllerCell == targetCell)
            {
                rowIndex = index;
                columnIndex = 1;
                return;
            }
        }
    }

    private RemapKeyCell GetCurrentCell()
    {
        if (rows.Count == 0)
        {
            return null;
        }

        RemapRow row = rows[currentRowIndex];

        return currentColumnIndex == 0
            ? row.keyboardCell
            : row.controllerCell;
    }

    private void ClampSelection()
    {
        if (rows.Count == 0)
        {
            currentRowIndex = 0;
            currentColumnIndex = 0;
            return;
        }

        currentRowIndex = Mathf.Clamp(
            currentRowIndex,
            0,
            rows.Count - 1
        );

        currentColumnIndex = Mathf.Clamp(
            currentColumnIndex,
            0,
            1
        );
    }

    private void UpdateHighlight()
    {
        for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
        {
            RemapRow row = rows[rowIndex];

            if (row.keyboardCell != null)
            {
                bool highlighted =
                    rowIndex == currentRowIndex &&
                    currentColumnIndex == 0;

                row.keyboardCell.SetHighlighted(highlighted);
            }

            if (row.controllerCell != null)
            {
                bool highlighted =
                    rowIndex == currentRowIndex &&
                    currentColumnIndex == 1;

                row.controllerCell.SetHighlighted(highlighted);
            }
        }
    }
}
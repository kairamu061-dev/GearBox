using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridUI : MonoBehaviour
{
    [SerializeField] GridCellUI cellPrefab;
    [SerializeField] RectTransform gridRoot;
    [SerializeField] float cellSize = 80f;
    [SerializeField] float cellSpacing = 4f;

    GridCellUI[,] cells;
    Vector2Int gridSize;

    public System.Action<Vector2Int> OnCellClicked;
    public System.Action<Vector2Int> OnCellHovered;

    public void Build(Vector2Int size)
    {
        gridSize = size;
        foreach (Transform child in gridRoot) Destroy(child.gameObject);

        cells = new GridCellUI[size.x, size.y];
        float step = cellSize + cellSpacing;
        float startX = -(size.x - 1) * step * 0.5f;
        float startY = -(size.y - 1) * step * 0.5f;

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var cell = Instantiate(cellPrefab, gridRoot);
                cell.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(startX + x * step, startY + y * step);
                int cx = x, cy = y;
                cell.OnClicked += () => OnCellClicked?.Invoke(new Vector2Int(cx, cy));
                cell.OnHovered += () => OnCellHovered?.Invoke(new Vector2Int(cx, cy));
                cells[x, y] = cell;
            }
        }
        Refresh();
    }

    public void Refresh()
    {
        var layout = RunManager.Instance.GridLayout;
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                cells[x, y].SetTower(layout[x, y]);
    }

    public void HighlightCell(Vector2Int pos, bool valid)
    {
        ClearHighlights();
        if (IsInBounds(pos)) cells[pos.x, pos.y].SetHighlight(valid);
    }

    public void ClearHighlights()
    {
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                cells[x, y].SetHighlight(null);
    }

    public bool IsInBounds(Vector2Int pos) =>
        pos.x >= 0 && pos.x < gridSize.x && pos.y >= 0 && pos.y < gridSize.y;

    public bool IsCellEmpty(Vector2Int pos) =>
        IsInBounds(pos) && RunManager.Instance.GridLayout[pos.x, pos.y] == null;

    public Vector2Int? GetCellUnderMouse()
    {
        for (int x = 0; x < gridSize.x; x++)
            for (int y = 0; y < gridSize.y; y++)
                if (RectTransformUtility.RectangleContainsScreenPoint(
                    cells[x, y].GetComponent<RectTransform>(),
                    Input.mousePosition, null))
                    return new Vector2Int(x, y);
        return null;
    }
}

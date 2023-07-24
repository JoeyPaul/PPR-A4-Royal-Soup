using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public int gridSizeX;           // Number of cells in the X-axis
    public int gridSizeY;           // Number of cells in the Y-axis
    public float cellSize;          // Size of each cell
    public Vector3 offset;          // Offset to apply to the grid position
    public Material gridMaterial;   // Material used for rendering the grid lines

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        Vector3 startingPosition = transform.position + offset;

        for (int x = 0; x <= gridSizeX; x++)
        {
            Vector3 start = startingPosition + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, gridSizeY * cellSize);
            DrawLine(start, end);
        }

        for (int y = 0; y <= gridSizeY; y++)
        {
            Vector3 start = startingPosition + new Vector3(0, 0, y * cellSize);
            Vector3 end = start + new Vector3(gridSizeX * cellSize, 0, 0);
            DrawLine(start, end);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject("GridLine");
        lineObject.transform.SetParent(transform);
        LineRenderer lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = gridMaterial;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}

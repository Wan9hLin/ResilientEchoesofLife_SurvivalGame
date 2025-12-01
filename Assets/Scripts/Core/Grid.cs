using UnityEngine;

public class Grid : MonoBehaviour
{
    private int _width;
    private int _length;
    private float _cellSize;
    private int[,] _gridArray;
    public static Grid Instance;
    public float height=0f;
    private void Awake()
    {
        Instance = this;
        _width = 100;
        _length = 100;
        _cellSize = 1.5f;
        CreateGrid();
    }

    private void CreateGrid()
    {
        _gridArray = new int[_width, _length];
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _length; j++)
            {
                Vector3 worldPosition = GetWorldPosition(i, j);

                // 绘制网格线条
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i, j + 1), Color.red, 100f);
                Debug.DrawLine(GetWorldPosition(i, j), GetWorldPosition(i + 1, j), Color.red, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, _length), GetWorldPosition(_width, _length), Color.red, 100f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _length), Color.red, 100f);
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x * _cellSize, height, z * _cellSize);
    }

    public void GetXYZ(Vector3 worldPosition, out int x, out int z,out float y)
    {
        x = Mathf.FloorToInt(worldPosition.x / _cellSize);
        z = Mathf.FloorToInt(worldPosition.z / _cellSize);
        y = height;
    }
}
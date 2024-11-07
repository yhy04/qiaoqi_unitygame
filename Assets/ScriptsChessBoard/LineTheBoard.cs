using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public float cellSize = 10; 
    private bool gridCreated = false;
    public LayerMask boardLayer; 
    public GameObject[,] cellObjects;
    public GameObject[,] placedPieces;
    public GameObject piecePrefab1;
    public GameObject piecePrefab2;
    public GameObject bridgePrefab;
    public int[,,] pieceConnect;

    public Color highlightColor = Color.yellow;
    public Color originalColor;
    public GameObject lastPlacedPiece;
    public int indexX,indexZ;
    public bool round = true;
    public bool canClickBoard = true; // 控制是否允许点击棋盘
    public GameObject[] highlightedPieces; // 存储被标亮的棋子

    private void Start()
    {
        Create();
    }

    private void Create()
    {
        if (gridCreated) return; // 如果网格已经创建，直接返回
        // 初始化二维数组
        cellObjects = new GameObject[width, height];
        placedPieces = new GameObject[width, height];
        highlightedPieces = new GameObject[8];
        pieceConnect = new int[width, height, 2];
        int ParentLayer = gameObject.layer;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // 创建单元格 GameObject
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.transform.position = new Vector3(x * cellSize - (width * cellSize) / 2 + cellSize / 2, 0,
                    z * cellSize - (height * cellSize) / 2 + cellSize / 2);
                cell.transform.localScale = new Vector3(cellSize, 0.1f, cellSize); // 使单元格扁平
                cell.AddComponent<BoxCollider>(); // 添加碰撞体
                cell.transform.parent = this.transform;
                cell.layer = ParentLayer;
                cellObjects[x, z] = cell;
                pieceConnect[x, z, 0] = 0;
                pieceConnect[x, z, 1] = 0;
            }
        }

        gridCreated = true; // 标记网格已创建
    }

    // 方法：根据给定的位置（x, z）访问并修改该位置的物体
    public GameObject GetCellObject(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return cellObjects[x, z];
        }
        return null; 
    }

    public GameObject GetPieceObject(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return placedPieces[x, z];
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white; // 设置网格线的颜色

        // 绘制水平线
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(-width * cellSize / 2, 0, y * cellSize - height * cellSize / 2);
            Vector3 end = new Vector3(width * cellSize / 2, 0, y * cellSize - height * cellSize / 2);
            Gizmos.DrawLine(start, end);
        }

        // 绘制垂直线
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize - width * cellSize / 2, 0, -height * cellSize / 2);
            Vector3 end = new Vector3(x * cellSize - width * cellSize / 2, 0, height * cellSize / 2);
            Gizmos.DrawLine(start, end);
        }
    }
}

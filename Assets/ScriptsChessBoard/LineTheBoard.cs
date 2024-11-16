using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 10;               //棋盘宽
    public int height = 10;              //棋盘长 
    public float cellSize = 10;          //每个棋格大小
    private bool gridCreated = false;    //判断是否创建了棋盘
    public LayerMask boardLayer;         //棋盘所处的层
    public GameObject[,] cellObjects;    //对每个棋格存储
    public GameObject[,] placedPieces;   //对每个已有的棋子存储
    public GameObject[,] placedBridge;   //记录桥
    public GameObject piecePrefab1;      //玩家1的棋子模型
    public GameObject piecePrefab2;      //玩家2的棋子模型
    public GameObject bridgePrefab;      //桥模型
    //public int[,,] pieceConnect;       //棋子相连情况记录
    public GameObject lastPlacedPiece;   //最后放置的棋子记录
    public int indexX,indexZ;            //最后放置的棋子在placedPieces中位置
    public bool round = true;
    public int[] parent1;
    public int[] rank1;
    public int[] parent2;
    public int[] rank2;
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
        placedBridge = new GameObject[width, height];
        parent1 = new int[width * height];
        rank1 = new int[width * height];
        parent2 = new int[width * height];
        rank2 = new int[width * height];
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
                parent1[x * height + z] = x * height + z;
                rank1[x * height + z] = 0;
                parent2[x * height + z] = x * height + z;
                rank2[x * height + z] = 0;
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

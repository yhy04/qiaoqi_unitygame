using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 8;  // 棋盘的宽度
    public int height = 8; // 棋盘的高度
    public float cellSize = 5.0f; // 每个单元格的大小

    private bool gridCreated = false; // 用于控制网格是否已经创建

    private void Start()
    {
        CreateGrid(); // 在开始时创建网格
    }

    private void CreateGrid()
    {
        if (gridCreated) return; // 如果网格已经创建，直接返回
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
                cell.name = $"Cell {x},{z}"; // 命名单元格
                cell.transform.parent = this.transform;
                cell.layer = ParentLayer;
            }
        }

        gridCreated = true; // 标记网格已创建
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

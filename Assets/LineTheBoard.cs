using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 8;  // 棋盘的宽度
    public int height = 8; // 棋盘的高度
    public float cellSize = 5.0f; // 每个单元格的大小

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

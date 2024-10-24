using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 8;  // ���̵Ŀ��
    public int height = 8; // ���̵ĸ߶�
    public float cellSize = 5.0f; // ÿ����Ԫ��Ĵ�С

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white; // ���������ߵ���ɫ

        // ����ˮƽ��
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(-width * cellSize / 2, 0, y * cellSize - height * cellSize / 2);
            Vector3 end = new Vector3(width * cellSize / 2, 0, y * cellSize - height * cellSize / 2);
            Gizmos.DrawLine(start, end);
        }

        // ���ƴ�ֱ��
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(x * cellSize - width * cellSize / 2, 0, -height * cellSize / 2);
            Vector3 end = new Vector3(x * cellSize - width * cellSize / 2, 0, height * cellSize / 2);
            Gizmos.DrawLine(start, end);
        }
    }
}

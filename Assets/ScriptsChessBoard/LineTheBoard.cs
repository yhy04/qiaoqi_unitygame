using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 8;  // ���̵Ŀ��
    public int height = 8; // ���̵ĸ߶�
    public float cellSize = 5.0f; // ÿ����Ԫ��Ĵ�С

    private bool gridCreated = false; // ���ڿ��������Ƿ��Ѿ�����

    private void Start()
    {
        CreateGrid(); // �ڿ�ʼʱ��������
    }

    private void CreateGrid()
    {
        if (gridCreated) return; // ��������Ѿ�������ֱ�ӷ���
        int ParentLayer = gameObject.layer;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // ������Ԫ�� GameObject
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.transform.position = new Vector3(x * cellSize - (width * cellSize) / 2 + cellSize / 2, 0,
                    z * cellSize - (height * cellSize) / 2 + cellSize / 2);
                cell.transform.localScale = new Vector3(cellSize, 0.1f, cellSize); // ʹ��Ԫ���ƽ
                cell.AddComponent<BoxCollider>(); // �����ײ��
                cell.name = $"Cell {x},{z}"; // ������Ԫ��
                cell.transform.parent = this.transform;
                cell.layer = ParentLayer;
            }
        }

        gridCreated = true; // ��������Ѵ���
    }

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

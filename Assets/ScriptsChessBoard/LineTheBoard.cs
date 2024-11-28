using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDrawer : MonoBehaviour
{
    public int width = 10;               //���̿�
    public int height = 10;              //���̳� 
    public float cellSize = 10;          //ÿ������С
    private bool gridCreated = false;    //�ж��Ƿ񴴽�������
    public LayerMask boardLayer;         //���������Ĳ�
    public GameObject[,] cellObjects;    //��ÿ�����洢
    public GameObject[,] placedPieces;   //��ÿ�����е����Ӵ洢
    public GameObject[,] placedBridge;   //��¼��
    public GameObject piecePrefab1;      //���1������ģ��
    public GameObject piecePrefab2;      //���2������ģ��
    public GameObject bridgePrefab;      //��ģ��
    //public int[,,] pieceConnect;       //�������������¼
    public GameObject lastPlacedPiece;   //�����õ����Ӽ�¼
    public int indexX,indexZ;            //�����õ�������placedPieces��λ��
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
        if (gridCreated) return; // ��������Ѿ�������ֱ�ӷ���
        // ��ʼ����ά����
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
                // ������Ԫ�� GameObject
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cell.transform.position = new Vector3(x * cellSize - (width * cellSize) / 2 + cellSize / 2, 0,
                    z * cellSize - (height * cellSize) / 2 + cellSize / 2);
                cell.transform.localScale = new Vector3(cellSize, 0.1f, cellSize); // ʹ��Ԫ���ƽ
                cell.AddComponent<BoxCollider>(); // ������ײ��
                cell.transform.parent = this.transform;
                cell.layer = ParentLayer;
                cellObjects[x, z] = cell;
                parent1[x * height + z] = x * height + z;
                rank1[x * height + z] = 0;
                parent2[x * height + z] = x * height + z;
                rank2[x * height + z] = 0;
            }
        }
        gridCreated = true; // ��������Ѵ���
    }

    // ���������ݸ�����λ�ã�x, z�����ʲ��޸ĸ�λ�õ�����
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

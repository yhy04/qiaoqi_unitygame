using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class ChessBoard : MonoBehaviour
{
    public LayerMask boardLayer; // ���̲��LayerMask
    public GameObject piecePrefab1; // ���ӵ�Prefab
    public GameObject piecePrefab2; // ���ӵ�Prefab
    public GameObject bridgePrefab;
    public float boardSize; // ���̴�С
    public float cellSize; // ��Ԫ���С
    public int cellNumber;

    public Color highlightColor = Color.yellow; // ͻ����ʾ����ɫ
    private Color originalColor; // ԭʼ��ɫ
    private GameObject lastPlacedPiece; // �����õ�����

    private bool[,] occupiedPositions; // ��¼ռ�����
    private bool round;

    private bool canClickBoard = true; // �����Ƿ�����������
    private GameObject[] highlightedPieces; // �洢������������

    void Start()
    {
        // ��ʼ��ռ��λ������
        occupiedPositions = new bool[cellNumber, cellNumber ];
        round = true;
        highlightedPieces = new GameObject[cellNumber ];
    }
    void Update()
    {
        
        if (canClickBoard && Input.GetMouseButtonDown(0))
            {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if (Physics.Raycast(ray, out hit))
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, boardLayer))
                {
                // ������λ���������ϵ�����
                Vector3 hitPoint = hit.point;
                int x = ToIndex(hitPoint.x);
                int z = ToIndex(hitPoint.z);

                // ȷ����������̷�Χ��
                if (hitPoint.x >= -boardSize && hitPoint.x < boardSize && hitPoint.z >= -boardSize && hitPoint.z < boardSize)
                {
                    // ʵ��������
                    if (!occupiedPositions[x + cellNumber / 2, z + cellNumber / 2])
                    {
                        GameObject newPiece;
                        occupiedPositions[x + cellNumber / 2, z + cellNumber / 2] = true;
                        Vector3 piecePosition = new Vector3((x + 0.5f) * cellSize, 3.60f, (z + 0.5f) * cellSize);
                        if (round)
                        {
                            newPiece = Instantiate(piecePrefab1, piecePosition, Quaternion.identity); 
                            newPiece.name = $"Piece1_{x}_{z}";
                            HighlightSurroundingPieces(1, x, z);
                        }
                        else
                        {
                            newPiece = Instantiate(piecePrefab2, piecePosition, Quaternion.identity);
                            newPiece.name = $"Piece2_{x}_{z}";
                            HighlightSurroundingPieces(2, x, z);
                        }
                        lastPlacedPiece = newPiece;
                        round = !round;
                        
                    }
                }
            }
        }
        else if (!canClickBoard && Input.GetMouseButtonDown(0)) // ֻ������ canClickBoard Ϊ false ʱ�������������
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // ��⵽�Ķ����Ǳ�����������
                
                if (hit.collider.CompareTag("HighlightedChessPiece"))
                {
                    // ִ�е����Ĳ���
                    Debug.Log("���������ӱ����: " + hit.collider.gameObject.name);

                    Vector3 selectedPiecePosition = hit.collider.transform.position;

                    Vector3 bridgePosition = (selectedPiecePosition + lastPlacedPiece.transform.position) / 2 + 
                        new Vector3(0 , 1f, 0 );

                    // ������ת��ʹ����ָ����������
                    Quaternion bridgeRotation = Quaternion.LookRotation(selectedPiecePosition - lastPlacedPiece.transform.position);

                    // ʵ��������ģ��
                    GameObject bridge = Instantiate(bridgePrefab, bridgePosition, bridgeRotation * Quaternion.Euler(90, 0, 0));


                    // �ָ����̵��
                    ResetHighlighting();
                    canClickBoard = true;
                }
            }
        }
    }
    private int ToIndex(float number)
    {
        // ���������
        if (number < 0)
        {
            return (int)(number / cellSize) - 1;
        }

        // ������������ȡ��
        double result = number / cellSize;
        return (int)Math.Floor(result);
    }

    private void HighlightSurroundingPieces(int n, int x, int z)
    {
        int index = 0;
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dz = -2; dz <= 2; dz++)
            {
                if (Math.Abs(dx) + Math.Abs(dz) != 3) continue;
                int newX = x + dx;
                int newZ = z + dz;

                // ȷ�������̷�Χ��
                if (newX >= -cellNumber/2 && newX < cellNumber / 2 && newZ >= -cellNumber / 2 && newZ < cellNumber / 2)
                {
                    // ͻ����ʾ��Χ������
                    GameObject piece = GetPieceAt(n, newX, newZ);
                    if (piece != null)
                    {
                        // ����ԭʼ��ɫ������Ϊͻ����ʾ��ɫ
                        Renderer renderer = piece.GetComponent<Renderer>();
                        originalColor = renderer.material.color; // ����ԭʼ��ɫ
                        renderer.material.color = highlightColor; // ����Ϊͻ����ʾ��ɫ
                        piece.tag = "HighlightedChessPiece";
                        highlightedPieces[index++] = piece;
                        canClickBoard = false;
                    }
                }
            }
        }
    }
    private void ResetHighlighting()
    {
        foreach (GameObject piece in highlightedPieces)
        {
            if (piece != null)
            {
                Renderer renderer = piece.GetComponent<Renderer>();
                renderer.material.color = originalColor; // �ָ�ԭʼ��ɫ
                piece.tag = "Untagged"; // �������Tag
            }
        }

        // ����ѱ�������������
        highlightedPieces = new GameObject[cellNumber];
    }
    private GameObject GetPieceAt(int nd, int x, int z)
    {
        // ����λ�÷������ӣ�����У�������ʹ�ñ�ǩ��������ʶ��
        return GameObject.Find($"Piece{nd}_{x}_{z}"); // ������������Ϊ "Piece_x_z"
    }
}


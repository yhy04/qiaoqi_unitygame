using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class ChessBoard : MonoBehaviour
{
    public LayerMask boardLayer; // 棋盘层的LayerMask
    public GameObject piecePrefab1; // 棋子的Prefab
    public GameObject piecePrefab2; // 棋子的Prefab
    public GameObject bridgePrefab;
    public float boardSize; // 棋盘大小
    public float cellSize; // 单元格大小
    public int cellNumber;

    public Color highlightColor = Color.yellow; // 突出显示的颜色
    private Color originalColor; // 原始颜色
    private GameObject lastPlacedPiece; // 最后放置的棋子

    private bool[,] occupiedPositions; // 记录占用情况
    private bool round;

    private bool canClickBoard = true; // 控制是否允许点击棋盘
    private GameObject[] highlightedPieces; // 存储被标亮的棋子

    void Start()
    {
        // 初始化占用位置数组
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
                // 计算点击位置在棋盘上的坐标
                Vector3 hitPoint = hit.point;
                int x = ToIndex(hitPoint.x);
                int z = ToIndex(hitPoint.z);

                // 确保点击在棋盘范围内
                if (hitPoint.x >= -boardSize && hitPoint.x < boardSize && hitPoint.z >= -boardSize && hitPoint.z < boardSize)
                {
                    // 实例化棋子
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
        else if (!canClickBoard && Input.GetMouseButtonDown(0)) // 只允许在 canClickBoard 为 false 时点击标亮的棋子
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 检测到的对象是被标亮的棋子
                
                if (hit.collider.CompareTag("HighlightedChessPiece"))
                {
                    // 执行点击后的操作
                    Debug.Log("标亮的棋子被点击: " + hit.collider.gameObject.name);

                    Vector3 selectedPiecePosition = hit.collider.transform.position;

                    Vector3 bridgePosition = (selectedPiecePosition + lastPlacedPiece.transform.position) / 2 + 
                        new Vector3(0 , 1f, 0 );

                    // 计算旋转，使桥梁指向两个棋子
                    Quaternion bridgeRotation = Quaternion.LookRotation(selectedPiecePosition - lastPlacedPiece.transform.position);

                    // 实例化桥梁模型
                    GameObject bridge = Instantiate(bridgePrefab, bridgePosition, bridgeRotation * Quaternion.Euler(90, 0, 0));


                    // 恢复棋盘点击
                    ResetHighlighting();
                    canClickBoard = true;
                }
            }
        }
    }
    private int ToIndex(float number)
    {
        // 处理负数情况
        if (number < 0)
        {
            return (int)(number / cellSize) - 1;
        }

        // 计算结果并向下取整
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

                // 确保在棋盘范围内
                if (newX >= -cellNumber/2 && newX < cellNumber / 2 && newZ >= -cellNumber / 2 && newZ < cellNumber / 2)
                {
                    // 突出显示周围的棋子
                    GameObject piece = GetPieceAt(n, newX, newZ);
                    if (piece != null)
                    {
                        // 保存原始颜色并设置为突出显示颜色
                        Renderer renderer = piece.GetComponent<Renderer>();
                        originalColor = renderer.material.color; // 保存原始颜色
                        renderer.material.color = highlightColor; // 设置为突出显示颜色
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
                renderer.material.color = originalColor; // 恢复原始颜色
                piece.tag = "Untagged"; // 清除特殊Tag
            }
        }

        // 清空已标亮的棋子数组
        highlightedPieces = new GameObject[cellNumber];
    }
    private GameObject GetPieceAt(int nd, int x, int z)
    {
        // 根据位置返回棋子（如果有），可以使用标签或名称来识别
        return GameObject.Find($"Piece{nd}_{x}_{z}"); // 假设棋子命名为 "Piece_x_z"
    }
}


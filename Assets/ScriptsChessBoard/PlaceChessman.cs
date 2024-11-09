using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class ChessBoard : MonoBehaviour
{

    public GridDrawer _;
   
    void Start()
    {
        for (int i = 0; i < _.width; i++) {
            Union1(0, i * _.height);
            Union1(_.height - 1, (i + 1) * _.height - 1);
        }
        for (int i = 0; i < _.height; i++)
        {
            Union2(0, i);
            Union2((_.width - 1) * _.height, (_.width - 1) * _.height + i);
        }
    }
    void Update()
    {
        if (_.canClickBoard && Input.GetMouseButtonDown(0))
            {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _.boardLayer))
                {
                // 计算点击位置在棋盘上的坐标
                Vector3 hitPoint = hit.point;
                
                int x = ToIndex(hitPoint.x);
                int z = ToIndex(hitPoint.z);

                // 确保点击在棋盘范围内
                if (x >= -_.width/2 && x < _.width/2 && z >= -_.height/2 && z < _.height/2)
                {
                    // 实例化棋子
                    if (!_.placedPieces[x + _.width / 2, z + _.height / 2])
                    {
                        GameObject newPiece;
                        Vector3 piecePosition = new Vector3((x + 0.5f) * _.cellSize, 0.0f, (z + 0.5f) * _.cellSize);
                        if (_.round)
                        {
                            newPiece = Instantiate(_.piecePrefab1, piecePosition, Quaternion.identity); 
                            newPiece.name = $"Piece1";
                            
                        }
                        else
                        {
                            newPiece = Instantiate(_.piecePrefab2, piecePosition, Quaternion.identity);
                            newPiece.name = $"Piece2";
                            //HighlightSurroundingPieces(newPiece, x + _.width / 2, z + _.height / 2);
                        }
                        newPiece.transform.parent=this.transform ;
                        _.placedPieces[x + _.width / 2, z + _.height / 2] = newPiece;
                        _.indexX = x + _.width / 2;
                        _.indexZ = z + _.height / 2;
                        _.lastPlacedPiece = newPiece;
                        _.round = !_.round;
                        HighlightSurroundingPieces(newPiece, x + _.width / 2, z + _.height / 2);
                    }
                }
            }
        }
        else if (!_.canClickBoard && Input.GetMouseButtonDown(0)) // 只允许在 canClickBoard 为 false 时点击标亮的棋子
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 检测到的对象是被标亮的棋子
                
                if (hit.collider.CompareTag("HighlightedChessPiece"))
                {
                    GameObject clickedObject = hit.collider.gameObject; // 获取点击的物体

                    for (int x = 0; x < _.width; x++)
                    {
                        for (int z = 0; z < _.height; z++)
                        {
                            if (_.placedPieces[x, z] == clickedObject)
                            {
                                _.pieceConnect[_.indexX, _.indexZ, 0] = x - _.indexX;
                                _.pieceConnect[_.indexX, _.indexZ, 1] = z - _.indexZ;
                                if (_.round)
                                {
                                    Union1(x * _.height + z, _.indexX * _.height + _.indexZ);
                                    if (Connected1(0, _.height - 1)) Debug.Log("win1");
                                }
                                else 
                                { 
                                    Union2(x * _.height + z, _.indexX * _.height + _.indexZ);
                                    if (Connected2(0, (_.width - 1) * _.height)) Debug.Log("win2");
                                }
                            }
                        }
                    }


                    Vector3 selectedPiecePosition = hit.collider.transform.position;
                    Vector3 bridgePosition = (selectedPiecePosition + _.lastPlacedPiece.transform.position) / 2 + 
                        new Vector3(0 , 1f, 0 );

                    // 计算旋转，使桥梁指向两个棋子
                    Quaternion bridgeRotation = Quaternion.LookRotation(selectedPiecePosition - _.lastPlacedPiece.transform.position);

                    // 实例化桥梁模型
                    GameObject bridge = Instantiate(_.bridgePrefab, bridgePosition, bridgeRotation * Quaternion.Euler(90, 0, 0));

                    // 恢复棋盘点击
                    ResetHighlighting();
                    _.canClickBoard = true;
                }
            }
        }
    }
    private int ToIndex(float number)
    {
        if (number < 0)
        {
            return (int)(number / _.cellSize) - 1;
        }
        double result = number / _.cellSize;
        return (int)Math.Floor(result);
    }

    private void HighlightSurroundingPieces(GameObject newpiece, int x, int z)    //标记棋子
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
                if (newX >= 0 && newX < _.width && newZ >= 0 && newZ < _.height)
                {
                    // 突出显示周围的棋子
                    GameObject piece = _.placedPieces[newX,newZ];
                    if (piece != null && piece.name == newpiece.name)
                    {
                        if (notBlock(x, z, newX, newZ))
                        {
                            // 保存原始颜色并设置为突出显示颜色
                            Renderer renderer = piece.GetComponent<Renderer>();
                            _.originalColor = renderer.material.color; // 保存原始颜色
                            renderer.material.color = _.highlightColor; // 设置为突出显示颜色
                            piece.tag = "HighlightedChessPiece";
                            _.highlightedPieces[index++] = piece;
                            _.canClickBoard = false;
                        }
                    }
                }
            }
        }
    }
    private void ResetHighlighting()    //恢复去除标记
    {
        foreach (GameObject piece in _.highlightedPieces)
        {
            if (piece != null)
            {
                Renderer renderer = piece.GetComponent<Renderer>();
                renderer.material.color = _.originalColor; // 恢复原始颜色
                piece.tag = "Untagged"; // 清除特殊Tag
            }
        }

        // 清空已标亮的棋子数组
        _.highlightedPieces = new GameObject[8];
    }

    private bool notBlock(int x,int z,int newX,int newZ) {
        int[][] point = new int[][]
        {
            new int[] { -1, 0 },
            new int[] { -1, 1 },
            new int[] { -1, 2 },
            new int[] { 0, 1 },
            new int[] { 0, 2 },
            new int[] { 0, 3 },
            new int[] { 1, -1 },
            new int[] { 1, 0 },
            new int[] { 1, 1 },
            new int[] { 2, 0 },
            new int[] { 2, 1 },
            new int[] { 2, 2 }
        };
        int[][,] array = new int[12][,]
        {
            new int[,] { { 2, 1 } },
            new int[,] { { 2, -1 } },
            new int[,] { { 2, -1 } },
            new int[,] { { 1, -2 }, { 2, 1 }, { 2, -1 } },
            new int[,] { { 1, -2 },{ 2, -1 } },
            new int[,] { { 1, -2 } },
            new int[,] { { -1, 2 } },
            new int[,] { { -1, 2 },{ -2, 1 } },
            new int[,] { { -1, 2 }, { -2, -1 }, { -2, 1 } },
            new int[,] { { -2, 1 } },
            new int[,] { { -2, 1 } },
            new int[,] { { -2, -1 } }
        };
        int[] funcarray = { 0, 0, 0, 0 };
        if (newX - x == 1 && newZ - z == 2)
        {
             funcarray= new int[4] { 1, 0, 0, 1 };
        }
        else if (newX - x == 1 && newZ - z == -2){
            funcarray = new int[4] { 1, 0, 0, -1 };
        }
        else if (newX - x == -1 && newZ - z == 2)
        {
            funcarray = new int[4] { -1, 0, 0, 1 };
        }
        else if (newX - x == -1 && newZ - z == -2)
        {
            funcarray = new int[4] { -1, 0, 0, -1 };
        }
        else if (newX - x == 2 && newZ - z == 1)
        {
            funcarray = new int[4] { 0, 1, 1, 0 };
        }
        else if (newX - x == 2 && newZ - z == -1)
        {
            funcarray = new int[4] { 0, -1, 1, 0 };
        }
        else if (newX - x == -2 && newZ - z == 1)
        {
            funcarray = new int[4] { 0, 1, -1, 0 };
        }
        else if (newX - x == -2 && newZ - z == -1)
        {
            funcarray = new int[4] { 0, -1, -1, 0 };
        }
        for (int i = 0; i < point.GetLength(0); i++)
        {
            if (!checkblock(point[i], array[i], x, z, funcarray)) return false;
        }
        return true; }

    private bool checkblock(int[] first, int[,] second,int x,int z, int[] funcarray) {
        Debug.Log(funcarray[0]);
        Debug.Log(funcarray[1]);
        Debug.Log(funcarray[2]);
        Debug.Log(funcarray[3]);
        for (int i = 0; i < second.GetLength(0); i++)
        {
            int a, b, c, d;
            a = first[0] * funcarray[0] + first[1] * funcarray[2];
            b = first[0] * funcarray[1] + first[1] * funcarray[3];
            if (x + a >= 0 && x + a < _.width && z + b >= 0 && z + b < _.height)
            {
                c = second[i, 0] * funcarray[0] + second[i, 1] * funcarray[2];
                d = second[i, 0] * funcarray[1] + second[i, 1] * funcarray[3];
                if (_.pieceConnect[x + a, z + b, 0] == c &&
                    _.pieceConnect[x + a, z + b, 1] == d) return false;
            }
        }
        return true;
    }

    private void Union1(int x, int y)
    {
        int rootX = Find1(x);
        int rootY = Find1(y);

        if (rootX != rootY)
        {
            // 按秩合并
            if (_.rank1[rootX] > _.rank1[rootY])
            {
                _.parent1[rootY] = rootX;
            }
            else if (_.rank1[rootX] < _.rank1[rootY])
            {
                _.parent1[rootX] = rootY;
            }
            else
            {
                _.parent1[rootY] = rootX;
                _.rank1[rootX]++;
            }
        }
    }

    private void Union2(int x, int y)
    {
        int rootX = Find2(x);
        int rootY = Find2(y);

        if (rootX != rootY)
        {
            // 按秩合并
            if (_.rank2[rootX] > _.rank2[rootY])
            {
                _.parent2[rootY] = rootX;
            }
            else if (_.rank2[rootX] < _.rank2[rootY])
            {
                _.parent2[rootX] = rootY;
            }
            else
            {
                _.parent2[rootY] = rootX;
                _.rank2[rootX]++;
            }
        }
    }

    private int Find1(int x)
    {
        if (_.parent1[x] != x)
        {
            _.parent1[x] = Find1(_.parent1[x]);
        }
        return _.parent1[x];
    }

    private int Find2(int x)
    {
        if (_.parent2[x] != x)
        {
            _.parent2[x] = Find2(_.parent2[x]);
        }
        return _.parent2[x];
    }

    private bool Connected1(int x, int y)
    {
        return Find1(x) == Find1(y);
    }

    private bool Connected2(int x, int y)
    {
        return Find2(x) == Find2(y);
    }
}


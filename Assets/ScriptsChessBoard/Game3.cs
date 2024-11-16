using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
public class Game3 : MonoBehaviour
{
    public GridDrawer _;
    void Start()
    {
        for (int i = 0; i < _.width; i++) {
            Union1(0, i * _.height);
            Union1(_.height - 1, (i + 1) * _.height - 1);
            _.cellObjects[i, 0].GetComponent<Renderer>().material.color = Color.red;
            _.cellObjects[i, _.height - 1].GetComponent<Renderer>().material.color = Color.red;
        }
        for (int i = 0; i < _.height; i++)
        {
            Union2(0, i);
            Union2((_.width - 1) * _.height, (_.width - 1) * _.height + i);
            _.cellObjects[0, i].GetComponent<Renderer>().material.color = Color.blue;
            _.cellObjects[_.width - 1, i].GetComponent<Renderer>().material.color = Color.blue;
        }
        _.cellObjects[0, 0].GetComponent<Renderer>().material.color = Color.white;
        _.cellObjects[0, _.height - 1].GetComponent<Renderer>().material.color = Color.white;
        _.cellObjects[_.width - 1, 0].GetComponent<Renderer>().material.color = Color.white;
        _.cellObjects[_.width - 1, _.height - 1].GetComponent<Renderer>().material.color = Color.white;
        int destroyCount = 20;
        for (int i = 0; i < destroyCount; i++)
        {
            // 随机选择 x 和 y
            int x = UnityEngine.Random.Range(0, _.width);
            int y = UnityEngine.Random.Range(0, _.height);

            // 获取随机位置的 GameObject
            GameObject obj = _.cellObjects[x, y];

            // 检查是否已经销毁，避免重复操作
            if (obj != null)
            {
                Destroy(obj); // 销毁对象
                _.cellObjects[x, y] = null; // 从数组中移除引用
            }
            else
            {
                // 如果随机到已经销毁的物体，重新尝试
                i--;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _.boardLayer))
                {
                // 计算点击位置在棋盘上的坐标
                Vector3 hitPoint = hit.point;
                int row, column;
                (row, column) = ToIndex(hitPoint);
                // 实例化棋子
                if (!_.placedPieces[row, column])
                {
                    GameObject newPiece;
                    Vector3 piecePosition = new Vector3((row - _.width / 2 + 0.5f) * _.cellSize, 0.0f, (column - _.height / 2 + 0.5f) * _.cellSize);
                    if (_.round)
                    {
                        newPiece = Instantiate(_.piecePrefab1, piecePosition, Quaternion.identity);
                        newPiece.name = $"Piece1";
                        _.cellObjects[row, column].GetComponent<Renderer>().material.color = Color.red;
                    }
                    else
                    {
                        newPiece = Instantiate(_.piecePrefab2, piecePosition, Quaternion.identity);
                        newPiece.name = $"Piece2";
                        _.cellObjects[row, column].GetComponent<Renderer>().material.color = Color.blue;
                    }
                    newPiece.transform.parent = this.transform;
                    _.placedPieces[row, column] = newPiece;
                    _.indexX = row;
                    _.indexZ = column;
                    _.lastPlacedPiece = newPiece;
                    _.round = !_.round;
                    ConnectSurroundingPieces(newPiece, row, column);
                }
            }
        }
    }
    private (int row,int column) ToIndex(Vector3 position)      //坐标转换为数组下标
    {
        return ((int)((position.x + _.width / 2 * _.cellSize) /_.cellSize), (int)((position.z + _.height / 2 * _.cellSize) / _.cellSize));
    }
    private void ConnectSurroundingPieces(GameObject newpiece, int x, int z)    //连接周围的棋子
    {
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
                            Vector3 Position1 = _.placedPieces[x,z].transform.position;
                            Vector3 Position2 = _.placedPieces[newX, newZ].transform.position;
                            // 计算旋转，使桥梁指向两个棋子
                            Quaternion bridgeRotation = Quaternion.LookRotation(Position1 - Position2);
                            // 实例化桥梁模型
                            GameObject bridge = Instantiate(_.bridgePrefab, (Position1 + Position2 + new Vector3(0, 1f, 0)) / 2, bridgeRotation * Quaternion.Euler(90, 0, 0));
                            _.placedBridge[x,z]=bridge;
                            _.placedBridge[newX,newZ] = bridge;
                            if (!_.round)
                            {
                                Union1(x * _.height + z, newX * _.height + newZ);
                                if (Connected1(0, _.height - 1)) Debug.Log("win1");
                            }
                            else
                            {
                                Union2(x * _.height + z, newX * _.height + newZ);
                                if (Connected2(0, (_.width - 1) * _.height)) Debug.Log("win2");
                            }
                        }
                    }
                }
            }
        }
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
        for (int i = 0; i < second.GetLength(0); i++)
        {
            int a, b, c, d;
            a = first[0] * funcarray[0] + first[1] * funcarray[2];
            b = first[0] * funcarray[1] + first[1] * funcarray[3];
            c = second[i, 0] * funcarray[0] + second[i, 1] * funcarray[2];
            d = second[i, 0] * funcarray[1] + second[i, 1] * funcarray[3];
            if (x + a >= 0 && x + a < _.width && z + b >= 0 && z + b < _.height &&
                x + a + c >= 0 && x + a + c < _.width && z + b + d >= 0 && z + b + d < _.height)
            {
                if (_.placedBridge[x + a, z + b] != null && _.placedBridge[x + a, z + b] == _.placedBridge[x + a + c, z + b + d]) return false;
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


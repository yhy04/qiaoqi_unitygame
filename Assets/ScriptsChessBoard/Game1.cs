using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
public class Game1 : MonoBehaviour
{
    public GridDrawer gridDrawer;
    private bool isRequestInProgress = false;
    void Start()
    {
        StartCoroutine(SendResetRequest());

        for (int i = 0; i < gridDrawer.width; i++) {
            Union1(0, i * gridDrawer.height);
            Union1(gridDrawer.height - 1, (i + 1) * gridDrawer.height - 1);
            gridDrawer.cellObjects[i, 0].GetComponent<Renderer>().material.color = Color.red;
            gridDrawer.cellObjects[i, gridDrawer.height - 1].GetComponent<Renderer>().material.color = Color.red;
        }
        for (int i = 0; i < gridDrawer.height; i++)
        {
            Union2(0, i);
            Union2((gridDrawer.width - 1) * gridDrawer.height, (gridDrawer.width - 1) * gridDrawer.height + i);
            gridDrawer.cellObjects[0, i].GetComponent<Renderer>().material.color = Color.blue;
            gridDrawer.cellObjects[gridDrawer.width - 1, i].GetComponent<Renderer>().material.color = Color.blue;
        }
        gridDrawer.cellObjects[0, 0].GetComponent<Renderer>().material.color = Color.white;
        gridDrawer.cellObjects[0, gridDrawer.height - 1].GetComponent<Renderer>().material.color = Color.white;
        gridDrawer.cellObjects[gridDrawer.width - 1, 0].GetComponent<Renderer>().material.color = Color.white;
        gridDrawer.cellObjects[gridDrawer.width - 1, gridDrawer.height - 1].GetComponent<Renderer>().material.color = Color.white;
    }

    void Update()
    {
        if (gridDrawer.round)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, gridDrawer.boardLayer))
                {
                    // 计算点击位置在棋盘上的坐标
                    Vector3 hitPoint = hit.point;
                    int row, column;
                    (row, column) = ConvertPositionToGridIndex(hitPoint);
                    if(row==0||row==gridDrawer.width-1)return;
                    // 实例化棋子
                    if (!gridDrawer.placedPieces[row, column])
                    {
                        GameObject newPiece;
                        Vector3 piecePosition = new Vector3((row - gridDrawer.width / 2 + 0.5f) * gridDrawer.cellSize, 0.0f, (column - gridDrawer.height / 2 + 0.5f) * gridDrawer.cellSize);
                        //if (gridDrawer.round)
                        //{
                            newPiece = Instantiate(gridDrawer.piecePrefab1, piecePosition, Quaternion.identity);
                            newPiece.name = $"Piece1";
                            gridDrawer.cellObjects[row, column].GetComponent<Renderer>().material.color = Color.red;
                        //}
                        //else
                        //{
                        //    newPiece = Instantiate(gridDrawer.piecePrefab2, piecePosition, Quaternion.identity);
                        //    newPiece.name = $"Piece2";
                        //    gridDrawer.cellObjects[row, column].GetComponent<Renderer>().material.color = Color.blue;
                        
                        newPiece.transform.parent = this.transform;
                        gridDrawer.placedPieces[row, column] = newPiece;
                        gridDrawer.indexX = row;
                        gridDrawer.indexZ = column;
                        gridDrawer.lastPlacedPiece = newPiece;
                        gridDrawer.round = !gridDrawer.round;
                        ConnectSurroundingPieces(newPiece, row, column);
                        isRequestInProgress = true;
                    }
                }
            }
        }
        else if (isRequestInProgress)
        {
            isRequestInProgress = false;
            //gridDrawer.round = !gridDrawer.round;//在发送请求后写过了
            StartCoroutine(SendPostRequest(gridDrawer.indexX, gridDrawer.indexZ));
        }
    }
    private (int row, int column) ConvertPositionToGridIndex(Vector3 position)
    {
        return ((int)((position.x + gridDrawer.width / 2 * gridDrawer.cellSize) / gridDrawer.cellSize), 
                (int)((position.z + gridDrawer.height / 2 * gridDrawer.cellSize) / gridDrawer.cellSize));
    }
    private void ConnectSurroundingPieces(GameObject newPiece, int x, int z)
    {
        // 预先计算可能的偏移量，避免嵌套循环
        (int dx, int dz)[] offsets = new[]
        {
            (-2, 1), (-2, -1),
            (-1, 2), (-1, -2),
            (1, 2), (1, -2),
            (2, 1), (2, -1)
        };

        foreach (var (dx, dz) in offsets)
        {
            int newX = x + dx;
            int newZ = z + dz;

            if (IsValidPosition(newX, newZ) && 
                gridDrawer.placedPieces[newX, newZ]?.name == newPiece.name &&
                notBlock(x, z, newX, newZ))
            {
                CreateBridge(x, z, newX, newZ);
            }
        }
    }
    private bool IsValidPosition(int x, int z)
    {
        return x >= 0 && x < gridDrawer.width && z >= 0 && z < gridDrawer.height;
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
            if (x + a >= 0 && x + a < gridDrawer.width && z + b >= 0 && z + b < gridDrawer.height &&
                x + a + c >= 0 && x + a + c < gridDrawer.width && z + b + d >= 0 && z + b + d < gridDrawer.height)
            {
                if (gridDrawer.placedBridge[x + a, z + b] != null && gridDrawer.placedBridge[x + a, z + b] == gridDrawer.placedBridge[x + a + c, z + b + d]) return false;
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
            if (gridDrawer.rank1[rootX] > gridDrawer.rank1[rootY])
            {
                gridDrawer.parent1[rootY] = rootX;
            }
            else if (gridDrawer.rank1[rootX] < gridDrawer.rank1[rootY])
            {
                gridDrawer.parent1[rootX] = rootY;
            }
            else
            {
                gridDrawer.parent1[rootY] = rootX;
                gridDrawer.rank1[rootX]++;
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
            if (gridDrawer.rank2[rootX] > gridDrawer.rank2[rootY])
            {
                gridDrawer.parent2[rootY] = rootX;
            }
            else if (gridDrawer.rank2[rootX] < gridDrawer.rank2[rootY])
            {
                gridDrawer.parent2[rootX] = rootY;
            }
            else
            {
                gridDrawer.parent2[rootY] = rootX;
                gridDrawer.rank2[rootX]++;
            }
        }
    }
    private int Find1(int x)
    {
        if (gridDrawer.parent1[x] != x)
        {
            gridDrawer.parent1[x] = Find1(gridDrawer.parent1[x]);
        }
        return gridDrawer.parent1[x];
    }
    private int Find2(int x)
    {
        if (gridDrawer.parent2[x] != x)
        {
            gridDrawer.parent2[x] = Find2(gridDrawer.parent2[x]);
        }
        return gridDrawer.parent2[x];
    }
    private bool Connected1(int x, int y)
    {
        return Find1(x) == Find1(y);
    }
    private bool Connected2(int x, int y)
    {
        return Find2(x) == Find2(y);
    }
    private void CreateBridge(int x1, int z1, int x2, int z2)
    {
        // 计算两个棋子的位置
        Vector3 position1 = gridDrawer.placedPieces[x1, z1].transform.position;
        Vector3 position2 = gridDrawer.placedPieces[x2, z2].transform.position;
        
        // 计算桥的中心位置（稍微抬高一点以避免与棋盘重叠）
        Vector3 bridgePosition = (position1 + position2) / 2 + new Vector3(0, 0.1f, 0);
        
        // 计算桥的旋转角度（使桥连接两个棋子）
        Vector3 direction = position2 - position1;
        Quaternion bridgeRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);
        
        // 创建桥
        GameObject bridge = Instantiate(gridDrawer.bridgePrefab, bridgePosition, bridgeRotation);
        
        // 设置桥的缩放（根据两点距离调整长度）
        float distance = Vector3.Distance(position1, position2);
        bridge.transform.localScale = new Vector3(2f, distance, 0.1f);
        
        // 设置桥的父对象
        bridge.transform.parent = transform;
        
        // 在网格中记录桥的位置
        gridDrawer.placedBridge[x1, z1] = bridge;
        gridDrawer.placedBridge[x2, z2] = bridge;

        // 更新游戏状态
        if (!gridDrawer.round)
        {
            Union1(x1 * gridDrawer.height + z1, x2 * gridDrawer.height + z2);
            // 检查玩家1是否获胜（连接上下边缘）
            if (Connected1(0, gridDrawer.height - 1))
            {
                Debug.Log("玩家1（红色）获胜！");
            }
        }
        else
        {
            Union2(x1 * gridDrawer.height + z1, x2 * gridDrawer.height + z2);
            // 检查玩家2是否获胜（连接左右边缘）
            if (Connected2(0, (gridDrawer.width - 1) * gridDrawer.height))
            {
                Debug.Log("玩家2（蓝色）获胜！");
            }
        }
    }

    [System.Serializable]
    public class MoveResponse
    {
        public string player_move;
        public string bot_move;
    }

    // 添加一个新的类用于序列化移动数据
    [System.Serializable]
    public class MoveData
    {
        public string move;
    }

    public IEnumerator SendPostRequest(int x, int y)
    {
        string move = $"{(char)('a' + x)}{y + 1}";
        // 使用新的 MoveData 类来创建 JSON
        var moveData = new MoveData { move = move };
        string jsonData = JsonUtility.ToJson(moveData);
        
        Debug.Log($"发送移动请求: {jsonData}");

        using (UnityWebRequest www = new UnityWebRequest("http://118.89.133.59:5740/move", "POST"))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                // 打印收到的响应
                Debug.Log($"收到服务器响应: {responseText}");
                
                MoveResponse response = JsonUtility.FromJson<MoveResponse>(responseText);
                
                // 解析机器人的移动位置
                if (!string.IsNullOrEmpty(response.bot_move))
                {
                    int botX = response.bot_move[0] - 'a';
                    int botY = int.Parse(response.bot_move.Substring(1)) - 1;
                    
                    // 放置机器人的棋子
                    PlacePieceAtPosition(botX, botY);
                }
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
            }
        }
    }

    // 添加新的辅助方法来放置棋子
    private void PlacePieceAtPosition(int row, int column)
    {
        if (!gridDrawer.placedPieces[row, column])
        {
            GameObject newPiece;
            Vector3 piecePosition = new Vector3(
                (row - gridDrawer.width / 2 + 0.5f) * gridDrawer.cellSize, 
                0.0f, 
                (column - gridDrawer.height / 2 + 0.5f) * gridDrawer.cellSize
            );

            newPiece = Instantiate(gridDrawer.piecePrefab2, piecePosition, Quaternion.identity);
            newPiece.name = "Piece2";
            gridDrawer.cellObjects[row, column].GetComponent<Renderer>().material.color = Color.blue;
            
            newPiece.transform.parent = this.transform;
            gridDrawer.placedPieces[row, column] = newPiece;
            gridDrawer.indexX = row;
            gridDrawer.indexZ = column;
            gridDrawer.lastPlacedPiece = newPiece;
            gridDrawer.round = !gridDrawer.round;
            ConnectSurroundingPieces(newPiece, row, column);
        }
    }

    private IEnumerator SendResetRequest()
    {
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://118.89.133.59:5740/reset", ""))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("重置请求成功");
            }
            else
            {
                Debug.LogError($"重置请求失败: {www.error}");
            }
        }
    }
}


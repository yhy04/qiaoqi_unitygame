using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{

    public void StartGame()
    {
        // 加载游戏场景，假设游戏场景的名称为 "GameScene"
        SceneManager.LoadScene("GameScene");
    }

    // 退出游戏的按钮
    public void QuitGame()
    {
        // 退出应用程序
        Application.Quit();

#if UNITY_EDITOR
        // 如果在编辑器中运行，则停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{

    public void StartGame()
    {
        // ������Ϸ������������Ϸ����������Ϊ "GameScene"
        SceneManager.LoadScene("GameScene");
    }

    // �˳���Ϸ�İ�ť
    public void QuitGame()
    {
        // �˳�Ӧ�ó���
        Application.Quit();

#if UNITY_EDITOR
        // ����ڱ༭�������У���ֹͣ����
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}

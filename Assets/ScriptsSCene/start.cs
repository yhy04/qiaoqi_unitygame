using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start : MonoBehaviour
{

    public void StartGame1()
    {
        SceneManager.LoadScene(1);
    }
    public void StartGame2()
    {
        SceneManager.LoadScene(2);
    }
    public void StartGame3()
    {
        SceneManager.LoadScene(3);
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

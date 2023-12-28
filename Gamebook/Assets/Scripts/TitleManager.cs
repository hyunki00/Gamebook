using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    GameObject startbutton;
    public ConversationManager conversationManager;

    public void StartButton()
    {
        PlayerPrefs.SetInt("CurrentID", 0);
        PlayerPrefs.SetInt("SaveID", 0);
        PlayerPrefs.SetInt("CurrentHP", 0);
        PlayerPrefs.SetInt("SaveHP", 0);

        // ingame ������ �̵�
        SceneManager.LoadScene("ingame");
    }

    public void Exitbutton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("������ �����մϴ�");
    }

    // Start is called before the first frame update
    public void ContinueButton()
    {


        // �� �񵿱������� �ε�
        StartCoroutine(LoadIngameSceneAsync());
    }

    IEnumerator LoadIngameSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ingame");

        // �ε��� �Ϸ�� ������ ���
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // ingame ������ �̵��� �Ŀ� OnSaveButtonClicked() ȣ��
        // (�̶��� ������ ����� ������ �ҷ��� ���Դϴ�)
        conversationManager.Continue();
        conversationManager.SaveGame();
    }
}
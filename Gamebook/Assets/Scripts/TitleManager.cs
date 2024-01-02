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

        // ingame 씬으로 이동
        SceneManager.LoadScene("ingame");
    }

    public void Exitbutton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("게임을 종료합니다");
    }

    // Start is called before the first frame update
    public void ContinueButton()
    {


        // 씬 비동기적으로 로드
        StartCoroutine(LoadIngameSceneAsync());
    }

    IEnumerator LoadIngameSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("ingame");

        // 로딩이 완료될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // ingame 씬으로 이동한 후에 OnSaveButtonClicked() 호출
        // (이때는 이전에 저장된 정보를 불러올 것입니다)
        conversationManager.Continue();
        conversationManager.SaveGame();
    }
}
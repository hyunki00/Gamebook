using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    GameObject startbutton;
    public ConversationManager conversationManager;
    public Image titleImage;
    Sequence TitleSequence;

    public void Awake()
    {
    }

    public void Start()
    {

        TitleSequence = DOTween.Sequence()
            .OnStart(() =>
            {
                transform.localScale = Vector3.zero;

            }
                )
            .Append(transform.DOScale(1, 1).SetEase(Ease.OutBounce))
            .Join(GetComponent<Image>().DOFade(1f, 1.5f))
            .SetDelay(0.5f);
        

    }

    public void StartButton()
    {
        PlayerPrefs.SetInt("CurrentID", 0);
        PlayerPrefs.SetInt("SaveID", 0);
        PlayerPrefs.SetInt("CurrentHP", 3);
        PlayerPrefs.SetInt("SaveHP", 3);

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
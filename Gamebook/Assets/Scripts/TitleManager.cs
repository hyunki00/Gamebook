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


    Vector3 EndSize = new Vector3(0.8f, 0.8f);
    Vector3 StartSize;

    public void Start()
    {

        TitleSequence = DOTween.Sequence();
        TitleSequence.Append(StartSequence());
        StartSize = titleImage.rectTransform.localScale;
    }
    Sequence StartSequence()
    {
        return DOTween.Sequence()
        .OnStart(() =>
        {
            titleImage.color = new Color(1f, 1f, 1f, 1f);

        }
            )

        .Append(titleImage.rectTransform.DOScale(EndSize,1f).SetEase(Ease.InOutCubic))
        .Join(titleImage.DOColor(new Color(1f, 1f, 1f, 0.8f), 1f))
        .SetDelay(0.5f)
        .SetLoops(-1, LoopType.Yoyo);
    }
    void OnEnable()
    {

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
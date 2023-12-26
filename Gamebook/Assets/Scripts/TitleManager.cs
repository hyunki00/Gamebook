using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
public class TitleManager : MonoBehaviour
{
    GameObject startbutton;
    public void StartButton()
    {
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


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        

    }
}

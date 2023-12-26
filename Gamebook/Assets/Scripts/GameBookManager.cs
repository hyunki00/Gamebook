using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBookManager : MonoBehaviour
{
    
    public Button titlebutton;
    // 페이지가 바뀌는 것을 인지하는 변수
    int pageChange;

    public void Start() 
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TitleCheckBox()
    {
        
    }

    public void Totitle()
    {
        
        SceneManager.LoadScene("Title");
    }
}

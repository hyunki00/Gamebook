using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBookManager : MonoBehaviour
{
    
    public Button titlebutton;
    // �������� �ٲ�� ���� �����ϴ� ����
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

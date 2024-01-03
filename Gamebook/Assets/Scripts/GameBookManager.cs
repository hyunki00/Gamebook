using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class GameBookManager : MonoBehaviour
{

    
    public Button titlebutton;
    public void Start()
    {

    }

    public void Totitle()
    {

        SceneManager.LoadScene("Title");
    }
}

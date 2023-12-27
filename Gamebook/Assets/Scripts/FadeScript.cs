using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static FadeScript;

public class FadeScript : MonoBehaviour
{
    public Image Panel;
    float time = 0f;
    float F_time = 1f;
    float F2_time = 0.3f;
    public void Fade()
    {
        StartCoroutine(FadeFlow());
            
    }
    IEnumerator FadeFlow()
    {
        Panel.gameObject.SetActive(true);
        time = 0f;
        Panel.color = Color.black;
        Color alpha = Panel.color;
        alpha.a = 0;
        
        while(alpha.a < 1f){
            
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0,1f,time);
            Panel.color = alpha;
            yield return null;
        }
        time = 0f;

        yield return new WaitForSeconds(1f);
        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1f, 0, time);
            Panel.color = alpha;
            yield return null;
        }

        Panel.gameObject.SetActive(false);
        yield return null;
    }
    public void white()
    {
        StartCoroutine(whiteFlow());

    }
    IEnumerator whiteFlow()
    {
        Panel.gameObject.SetActive(true);
        time = 0f;
        Panel.color = Color.white;
       
        Color alpha = Panel.color;
        alpha.a = 0;

        
        while (alpha.a < 1f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1f, time);
            Panel.color = alpha;
            yield return null;
        }
        time = 0f;

        yield return new WaitForSeconds(1f);
        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1f, 0, time);
            Panel.color = alpha;
            yield return null;
        }

        Panel.gameObject.SetActive(false);
        yield return null;
    }

    public void red()
    {
        StartCoroutine(RedFlow());

    }
    IEnumerator RedFlow()
    {

        Panel.gameObject.SetActive(true);
        time = 0f;
        Panel.color = Color.red;
    
        Color alpha = Panel.color;
        alpha.a = 0;

        
        while (alpha.a < 0.3f)
        {
            time += Time.deltaTime / F2_time;
            alpha.a = Mathf.Lerp(0, 0.3f, time);
            Panel.color = alpha;
            yield return null;
        }
        time = 0f;

        
        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F2_time;
            alpha.a = Mathf.Lerp(0.3f, 0, time);
            Panel.color = alpha;
            yield return null;
        }

        Panel.gameObject.SetActive(false);
        yield return null;
    }
    public void Green()
    {
        StartCoroutine(GreenFlow());

    }

    IEnumerator GreenFlow()
    {
        Panel.gameObject.SetActive(true);
        time = 0f;
        Panel.color = Color.green;
        Color alpha = Panel.color;
        alpha.a = 0;
   
        while (alpha.a < 0.3f)
        {
            time += Time.deltaTime / F2_time;
            alpha.a = Mathf.Lerp(0, 0.3f, time);
            Panel.color = alpha;
            yield return null;
        }
        time = 0f;

       
        while (alpha.a > 0f)
        {
            time += Time.deltaTime / F2_time;
            alpha.a = Mathf.Lerp(0.3f, 0, time);
            Panel.color = alpha;
            yield return null;
        }

        Panel.gameObject.SetActive(false);
        yield return null;
    }
}


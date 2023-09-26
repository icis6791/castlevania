using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RawImage lkj;
    public RawImage back;
    public CanvasGroup startBtn;
    public CanvasGroup exitBtn;
    public Text titleTxt;
    public GameObject panel;
    public GameObject inputPanel;
    float t1;
    float t2;
    float curr = 120;

    void Start()
    {
        back.color = new Color(1, 1, 1, 1);
        lkj.color = new Color(1, 1, 1, 1);
        startBtn.alpha = 0;
        startBtn.interactable = false;
        startBtn.blocksRaycasts = false;
        exitBtn.alpha = 0;
        exitBtn.interactable = false;
        exitBtn.blocksRaycasts = false;
        titleTxt.color = new Color(titleTxt.color.r, titleTxt.color.g, titleTxt.color.b, 0);

        StartCoroutine(Blink());
    }

    void Update()
    {
        t1 -= Time.deltaTime * 50;
        t2 += Time.deltaTime * 50;

        if (Input.GetMouseButton(0))
        {
            StopAllCoroutines();
            back.color = new Color(1, 1, 1, 1);
            titleTxt.color = new Color(0, 1, 0, 1);
            startBtn.alpha = 1;
            exitBtn.alpha = 1;

            startBtn.interactable = true;
            startBtn.blocksRaycasts = true;
            startBtn.interactable = true;
            startBtn.blocksRaycasts = true;
            exitBtn.interactable = true;
            exitBtn.blocksRaycasts = true;
        }
    }

    IEnumerator Blink()
    {
        Color backcolor = back.color;
        Color titlecolor = titleTxt.color;
        yield return new WaitForSeconds(2);

        t1 = 120;
        while (true)
        {
            backcolor.a = t1 / curr;
            back.color = backcolor;
            yield return null;
            if( t1 <= 0)
            {
                t1 = 0;
                break;
            }
        }

        yield return new WaitForSeconds(0.5f);

        t2 = 0;
        while (true)
        {
            backcolor.a = t2 / curr;
            back.color = backcolor;
            yield return null;
            if( t2 >= curr)
            {
                t2 = curr;
                break;
            }
        }

        t2 = 0;
        while(true)
        {
            startBtn.alpha = t2 / 80;
            exitBtn.alpha = t2 / 80;
            titlecolor.a = t2 / 80;
            titleTxt.color = titlecolor;
            yield return null;
            if(t2 >= 80)
            {
                t2 = 80;
                startBtn.interactable = true;
                startBtn.blocksRaycasts = true;
                exitBtn.interactable = true;
                exitBtn.blocksRaycasts = true;
                break;
            }
        }
    }

    public void Show()
    {
        panel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        panel.gameObject.SetActive(false);
        inputPanel.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    public GameObject gameOver;  //게임 오버 화면
    public GameObject hpIcon;  //체력증가 아이콘
    public GameObject forceIcon;  //공격증가 아이콘
    public GameObject jumpIcon;  //2단점프 아이콘
    public GameObject saveMsg;  //저장완료 메시지
    public GameObject hpMsg;  //최대체력 증가 메시지
    public GameObject forceMsg;  //공격력 증가 메시지
    public GameObject jumpMsg;  //2단점프 가능 메시지
    public GameObject menu;  //메뉴창
    public GameObject blackImg;  //화면 가리기용 이미지

    public Image hpBar;  //체력바
    public bool isMenuOpen;

    WaitForSeconds ws;

    private void Start()
    {
        ws = new WaitForSeconds(1);
        StartCoroutine(BlackImg());
    }

    IEnumerator BlackImg()
    {
        if(DataManager.instance.nowPlayer.healthGain)
        {
            hpIcon.SetActive(true);
        }
        if(DataManager.instance.nowPlayer.forceGain)
        {
            forceIcon.SetActive(true);
        }
        if(DataManager.instance.nowPlayer.doubleJumpSkill)
        {
            jumpIcon.SetActive(true);
        }

        yield return new WaitForSeconds(0.25f);
        blackImg.SetActive(false);
    }

    public void SetHUD(float fillAmount)
    {
        hpBar.fillAmount = fillAmount;
    }

    public void ShowSave()
    {
        StartCoroutine(_ShowSave());
    }

    IEnumerator _ShowSave()
    {
        saveMsg.SetActive(true);
        yield return ws;
        saveMsg.SetActive(false);
    }

    public void ShowForce()
    {
        StartCoroutine(_ShowForce());
    }

    IEnumerator _ShowForce()
    {
        forceMsg.SetActive(true);
        forceIcon.SetActive(true);
        yield return ws;
        forceMsg.SetActive(false);
    }

    public void ShowHealth()
    {
        StartCoroutine(_ShowHealth());
    }

    IEnumerator _ShowHealth()
    {
        hpMsg.SetActive(true);
        hpIcon.SetActive(true);
        yield return ws;
        hpMsg.SetActive(false);
    }

    public void ShowJump()
    {
        StartCoroutine(_ShowJump());
    }

    IEnumerator _ShowJump()
    {
        jumpMsg.SetActive(true);
        jumpIcon.SetActive(true);
        yield return ws;
        jumpMsg.SetActive(false);
    }

    public void OpenMenu()  //메뉴를 보이는 메서드
    {
        if (Input.GetButtonDown("Cancel"))
        {
            menu.gameObject.SetActive(true);
            Time.timeScale = 0;
            isMenuOpen = true;
        }
    }

    public void CloseMenu()  //메뉴를 닫는 메서드
    {
        if (Input.GetButtonDown("Cancel"))
        {
            menu.gameObject.SetActive(false);
            Time.timeScale = 1;
            isMenuOpen = false;
        }
    }

    public void GoTitleScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}

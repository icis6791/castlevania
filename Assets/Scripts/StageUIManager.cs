using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageUIManager : MonoBehaviour
{
    public GameObject gameOver;  //���� ���� ȭ��
    public GameObject hpIcon;  //ü������ ������
    public GameObject forceIcon;  //�������� ������
    public GameObject jumpIcon;  //2������ ������
    public GameObject saveMsg;  //����Ϸ� �޽���
    public GameObject hpMsg;  //�ִ�ü�� ���� �޽���
    public GameObject forceMsg;  //���ݷ� ���� �޽���
    public GameObject jumpMsg;  //2������ ���� �޽���
    public GameObject menu;  //�޴�â
    public GameObject blackImg;  //ȭ�� ������� �̹���

    public Image hpBar;  //ü�¹�
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

    public void OpenMenu()  //�޴��� ���̴� �޼���
    {
        if (Input.GetButtonDown("Cancel"))
        {
            menu.gameObject.SetActive(true);
            Time.timeScale = 0;
            isMenuOpen = true;
        }
    }

    public void CloseMenu()  //�޴��� �ݴ� �޼���
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

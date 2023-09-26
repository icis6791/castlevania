using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject playerPrefab;
    public List<GameObject> fireObjects = new List<GameObject>();

    public bool isGameOver = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(instance.gameObject);
    }

    public void LoadTitleScene()  //타이틀 씬 호출
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void LoadBattleScene()
    {
        StartCoroutine(LoadBattle());
    }

    IEnumerator LoadBattle()
    {
        SceneManager.LoadScene(1);
        yield return new WaitForSeconds(0.1f);
        GameObject player = Instantiate(playerPrefab);
    }

    public void Erase()
    {
        for(int i = 0; i< fireObjects.Count; i++)
        {
            if (fireObjects[i] == null)
            {
                continue;
            }
            Destroy(fireObjects[i].gameObject);
        }
        fireObjects.Clear();
    }
}

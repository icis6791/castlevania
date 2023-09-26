using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Select : MonoBehaviour
{
    public GameObject create;
    public Text[] slotText;
    public Text newPlayerName;

    bool[] savefile = new bool[3];

    void Start()
    {
        //슬롯별로 저장된 데이터가 존재하는지 판단.
        for (int i = 0; i < 3; i++)
        {
            if (File.Exists(DataManager.instance.path + $"{i}"))
            {
                savefile[i] = true;
                DataManager.instance.nowSlot = i;
                DataManager.instance.LoadData();
                slotText[i].text = DataManager.instance.nowPlayer.name;
            }
            else
            {
                slotText[i].text = "비어있음";
            }
        }
        DataManager.instance.DataClear();
    }

    public void Slot(int number)
    {
        DataManager.instance.nowSlot = number;

        if (savefile[number])
        {
            DataManager.instance.LoadData();
            GoGame();
        }
        else
        {
            Create();
        }
    }

    public void Create()
    {
        create.gameObject.SetActive(true);
    }

    public void GoGame()
    {
        if (!savefile[DataManager.instance.nowSlot])
        {
            DataManager.instance.nowPlayer.name = newPlayerName.text;
        }

        if (DataManager.instance.nowPlayer.name.Length >= 1)
        {
            GameManager.instance.LoadBattleScene();
        }
    }
}

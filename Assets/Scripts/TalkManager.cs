using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    void Start()
    {
        talkData = new Dictionary<int, string[]>();
    }

    void GenerateData()
    {
        //id = 100 : �÷��̾�
        talkData.Add(100, new string[] { "��� ������� �����̴�.", "������̴� �� �� ���� �� ����." });
        //id = 200 : Ʃ�丮��
        talkData.Add(200, new string[] { "����� ���̴�. \n�� �� ���� �� ����" });
    }

    public string GetTalk(int id, int talkIndex) //Object�� id , string�迭�� index
    {
        return talkData[id][talkIndex]; //�ش� ���̵��� �ش�
    }
}

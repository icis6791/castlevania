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
        //id = 100 : 플레이어
        talkData.Add(100, new string[] { "쇠로 만들어진 감옥이다.", "열쇠없이는 열 수 없는 것 같다." });
        //id = 200 : 튜토리얼
        talkData.Add(200, new string[] { "평범한 문이다. \n들어갈 수 있을 것 같다" });
    }

    public string GetTalk(int id, int talkIndex) //Object의 id , string배열의 index
    {
        return talkData[id][talkIndex]; //해당 아이디의 해당
    }
}

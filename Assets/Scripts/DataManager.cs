
using UnityEngine;
using System.IO;
using System;

//저장하는 방법
//1. 저장할 데이터가 존재
//2. 데이터를 제이슨으로 변환
//3. 제이슨을 외부에 저장

//불러오는 방법
//1. 외부에 저장된 제이슨을 가져옴
//2. 제이슨을 데이터형태로 변환
//3. 불러온 데이터를 사용

//저장해야할 변수들을 가진 클래스
public class PlayerData
{
    public string name;
    public Vector3 playerTr;
    public bool bossComa;
    public bool bossComb;
    public bool healthGain;
    public bool forceGain;
    public bool doubleJumpSkill;
    public int damage;
    public float iniHp;
}

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    //PlayerData 형태의 nowPlayer 할당
    public PlayerData nowPlayer = new PlayerData();
    public string path; //저장 경로
    public int nowSlot;  //현재 슬롯

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
        }

        //Application.persistentDataPath 유니티에서 알아서 설정하는 경로.
        //path = Application.persistentDataPath + "/";
        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/";
        DontDestroyOnLoad(instance.gameObject);
    }

    public void SaveData()
    {
        //데이터를 제이슨 형태로 변환
        string data = JsonUtility.ToJson(nowPlayer);

        //경로에 파일 저장.
        File.WriteAllText(path + nowSlot.ToString(), data);
    }

    public void LoadData()
    {
        //경로에서 파일 가져오기
        string data = File.ReadAllText(path + nowSlot.ToString());

        //제이슨형태에서 데이터 형태로 변환.
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
    }

    public void DataClear()
    {
        nowSlot = -1;
        nowPlayer = new PlayerData();
    }
}


using UnityEngine;
using System.IO;
using System;

//�����ϴ� ���
//1. ������ �����Ͱ� ����
//2. �����͸� ���̽����� ��ȯ
//3. ���̽��� �ܺο� ����

//�ҷ����� ���
//1. �ܺο� ����� ���̽��� ������
//2. ���̽��� ���������·� ��ȯ
//3. �ҷ��� �����͸� ���

//�����ؾ��� �������� ���� Ŭ����
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

    //PlayerData ������ nowPlayer �Ҵ�
    public PlayerData nowPlayer = new PlayerData();
    public string path; //���� ���
    public int nowSlot;  //���� ����

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

        //Application.persistentDataPath ����Ƽ���� �˾Ƽ� �����ϴ� ���.
        //path = Application.persistentDataPath + "/";
        path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/";
        DontDestroyOnLoad(instance.gameObject);
    }

    public void SaveData()
    {
        //�����͸� ���̽� ���·� ��ȯ
        string data = JsonUtility.ToJson(nowPlayer);

        //��ο� ���� ����.
        File.WriteAllText(path + nowSlot.ToString(), data);
    }

    public void LoadData()
    {
        //��ο��� ���� ��������
        string data = File.ReadAllText(path + nowSlot.ToString());

        //���̽����¿��� ������ ���·� ��ȯ.
        nowPlayer = JsonUtility.FromJson<PlayerData>(data);
    }

    public void DataClear()
    {
        nowSlot = -1;
        nowPlayer = new PlayerData();
    }
}

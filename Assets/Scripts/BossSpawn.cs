using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawn : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnTr;

    void Start()
    {
        //Invoke("CreateBoss", 5);
    }

    void CreateBoss()
    {
        if (bossPrefab != null)
        {
            var boss = Instantiate(bossPrefab, spawnTr.position, spawnTr.rotation);
        }
    }
}

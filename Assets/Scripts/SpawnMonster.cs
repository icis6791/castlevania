using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  //몬스터 프리팹들 배열
    List<GameObject> enemyList = new List<GameObject>();  //생성하면 넣을 리스트
    public Transform[] SpawnPoints;  //생성 위치 배열

    int random;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //플레이어가 충돌하고 트리거가 Off이면
        if (collision.CompareTag("PLAYER") && !collision.isTrigger)
        {
            //몬스터 생성.
            for(int i = 0; i < SpawnPoints.Length; i++)
            {
                Create(SpawnPoints[i]);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //플레이어가 충돌하고 트리거가 Off이면
        if (collision.CompareTag("PLAYER") && !collision.isTrigger)
        {
            //몬스터 삭제.
            foreach (GameObject enemy in enemyList)
            {
                Destroy(enemy);
            }
            enemyList.Clear();
        }
    }

    void Create(Transform SpawnPoint)
    {
        random = Random.Range(0, enemyPrefabs.Length);
        GameObject enemyPrefab = enemyPrefabs[random];

        // 이미 생성된 몬스터 리스트에서 같은 프리팹을 가진 몬스터를 찾음.
        GameObject existingEnemy = enemyList.Find(enemy => enemy.CompareTag(enemyPrefab.tag));

        if (existingEnemy != null)
        {
            // 이미 같은 프리팹을 가진 몬스터가 존재하므로 다시 생성.
            Create(SpawnPoint);
            return;
        }

        var enemy = Instantiate(enemyPrefab, SpawnPoint.position, SpawnPoint.rotation);
        enemyList.Add(enemy);
    }
}

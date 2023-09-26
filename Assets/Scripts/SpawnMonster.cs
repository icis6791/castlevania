using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  //���� �����յ� �迭
    List<GameObject> enemyList = new List<GameObject>();  //�����ϸ� ���� ����Ʈ
    public Transform[] SpawnPoints;  //���� ��ġ �迭

    int random;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //�÷��̾ �浹�ϰ� Ʈ���Ű� Off�̸�
        if (collision.CompareTag("PLAYER") && !collision.isTrigger)
        {
            //���� ����.
            for(int i = 0; i < SpawnPoints.Length; i++)
            {
                Create(SpawnPoints[i]);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //�÷��̾ �浹�ϰ� Ʈ���Ű� Off�̸�
        if (collision.CompareTag("PLAYER") && !collision.isTrigger)
        {
            //���� ����.
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

        // �̹� ������ ���� ����Ʈ���� ���� �������� ���� ���͸� ã��.
        GameObject existingEnemy = enemyList.Find(enemy => enemy.CompareTag(enemyPrefab.tag));

        if (existingEnemy != null)
        {
            // �̹� ���� �������� ���� ���Ͱ� �����ϹǷ� �ٽ� ����.
            Create(SpawnPoint);
            return;
        }

        var enemy = Instantiate(enemyPrefab, SpawnPoint.position, SpawnPoint.rotation);
        enemyList.Add(enemy);
    }
}

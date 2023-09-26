using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KsSpawn : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnTr;
    PlayerCtrl player;
    public BoxCollider2D boxCol1;
    public BoxCollider2D boxCol2;
    Karas karas;

    bool done = false; //�����Ϸ�

    private void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            player = other.gameObject.GetComponent<PlayerCtrl>();
            if (!player.bossComB)
            {
                //���� ����.
                StartCoroutine(CreateBoss());
            }
        }
    }

    //ī�� ���� �ڷ�ƾ
    IEnumerator CreateBoss()
    {
        if (bossPrefab != null)
        {
            var boss = Instantiate(bossPrefab, spawnTr.position, spawnTr.rotation);
            done = true;
            karas = GameObject.Find("Karas(Clone)").GetComponent<Karas>();
            //karas.transform.position = new Vector3(40, -9, 0);
            yield return new WaitForSeconds(0.2f);
            //�� �� �Ա� ����.
            boxCol1.enabled = true;
            boxCol2.enabled = true;
            yield break;
        }
    }

    private void Update()
    {
        if (done)
        {
            if (karas.isDie)
            {
                //ī�󽺰� ������ �� �ٽ� ������.
                boxCol1.enabled = false;
                boxCol2.enabled = false;
            }
        }
    }
}

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

    bool done = false; //생성완료

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
                //몬스터 생성.
                StartCoroutine(CreateBoss());
            }
        }
    }

    //카라스 생성 코루틴
    IEnumerator CreateBoss()
    {
        if (bossPrefab != null)
        {
            var boss = Instantiate(bossPrefab, spawnTr.position, spawnTr.rotation);
            done = true;
            karas = GameObject.Find("Karas(Clone)").GetComponent<Karas>();
            //karas.transform.position = new Vector3(40, -9, 0);
            yield return new WaitForSeconds(0.2f);
            //양 쪽 입구 막기.
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
                //카라스가 죽으면 문 다시 열리기.
                boxCol1.enabled = false;
                boxCol2.enabled = false;
            }
        }
    }
}

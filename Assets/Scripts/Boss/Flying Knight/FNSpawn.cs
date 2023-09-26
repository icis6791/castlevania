using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNSpawn : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform spawnTr;
    PlayerCtrl player;
    PolygonCollider2D polyCol;
    public BoxCollider2D boxCol1;
    public BoxCollider2D boxCol2;
    FlyingKnight flyingKnight;

    bool done = false;

    private void Start()
    {
        polyCol = GetComponent<PolygonCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            player = other.gameObject.GetComponent<PlayerCtrl>();
            if(!player.bossComA)
            {
                //몬스터 생성.
                StartCoroutine(CreateBoss());
            }
        }
    }

    IEnumerator CreateBoss()
    {
        if (bossPrefab != null)
        {
            var boss = Instantiate(bossPrefab, spawnTr.position, spawnTr.rotation);
            done = true;
            flyingKnight = GameObject.Find("Flying Knight(Clone)").GetComponent<FlyingKnight>();
            flyingKnight.transform.position = new Vector3(40, -9, 0);
            boxCol1.enabled = true;
            boxCol2.enabled = true;
            yield break;
        }
    }

    private void Update()
    {
        if(done)
        {
            if (flyingKnight.isDie)
            {
                boxCol1.enabled = false;
                boxCol2.enabled = false;
            }
        }
    }
}

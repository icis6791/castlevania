using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSpawn : MonoBehaviour
{
    public GameObject itemPrefab;
    List<GameObject> itemList = new List<GameObject>();
    public Transform spawnPoint;
    PlayerCtrl player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어가 충돌하고 트리거가 Off이면
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            player = other.gameObject.GetComponent<PlayerCtrl>();
            if(!player.forceGain)
            {
                //아이템 생성.
                Create();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //플레이어가 충돌하고 트리거가 Off이면
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            //아이템 삭제.
            foreach (GameObject item in itemList)
            {
                Destroy(item);
            }
        }
    }

    void Create()
    {
        var item = Instantiate(itemPrefab, spawnPoint.position, spawnPoint.rotation);
        itemList.Add(item);
    }
}

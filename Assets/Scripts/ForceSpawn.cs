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
        //�÷��̾ �浹�ϰ� Ʈ���Ű� Off�̸�
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            player = other.gameObject.GetComponent<PlayerCtrl>();
            if(!player.forceGain)
            {
                //������ ����.
                Create();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //�÷��̾ �浹�ϰ� Ʈ���Ű� Off�̸�
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            //������ ����.
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

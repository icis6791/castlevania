using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erase : MonoBehaviour
{
    //무기 스폰위치
    public Transform spawnA;
    public Transform spawnB;

    private void OnTriggerExit2D(Collider2D other)
    {
        //무기가 트리거에서 떨어지면 위치를 스폰위치 위치와 회전으로 바꿈.
        if(other.CompareTag("BOSSWP_A"))
        {
            //print("충돌 판정");
            other.transform.position = spawnA.position;
            other.transform.rotation = spawnA.rotation;
        }
        if(other.CompareTag("BOSSWP_B"))
        {
            //print("충돌 판정");
            other.transform.position = spawnB.position;
            other.transform.rotation = spawnB.rotation;
        }
    }
}

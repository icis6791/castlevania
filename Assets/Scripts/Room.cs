using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCam;  //시네머신 팔로우 카메라

    //방 마다 Room스크립트와 콜라이더를 만들고, 그 콜라이더에 들어오면 그 방의 카메라를 키고 그 전까지 있던
    //방의 카메라를 끄는 것임.

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어가 충돌하고 트리거가 Off이면
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            //카메라 활성화
            virtualCam.SetActive(true);
            GameManager.instance.Erase();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //플레이어가 충돌하고 트리거가 Off이면
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            //카메라 비활성화
            virtualCam.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject virtualCam;  //�ó׸ӽ� �ȷο� ī�޶�

    //�� ���� Room��ũ��Ʈ�� �ݶ��̴��� �����, �� �ݶ��̴��� ������ �� ���� ī�޶� Ű�� �� ������ �ִ�
    //���� ī�޶� ���� ����.

    private void OnTriggerEnter2D(Collider2D other)
    {
        //�÷��̾ �浹�ϰ� Ʈ���Ű� Off�̸�
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            //ī�޶� Ȱ��ȭ
            virtualCam.SetActive(true);
            GameManager.instance.Erase();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //�÷��̾ �浹�ϰ� Ʈ���Ű� Off�̸�
        if (other.CompareTag("PLAYER") && !other.isTrigger)
        {
            //ī�޶� ��Ȱ��ȭ
            virtualCam.SetActive(false);
        }
    }
}

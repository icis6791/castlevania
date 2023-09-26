using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpGain : MonoBehaviour
{
    StageUIManager SUIMng;
    void Start()
    {
        SUIMng = GameObject.FindWithTag("MANAGER").GetComponent<StageUIManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("PLAYER"))
        {
            PlayerCtrl player = other.gameObject.GetComponent<PlayerCtrl>();
            player.doubleJumpSkill = true;
            SUIMng.ShowJump();
            Destroy(this.gameObject);
        }
    }
}

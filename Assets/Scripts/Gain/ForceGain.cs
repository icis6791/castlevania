using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGain : MonoBehaviour
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
            player.attack.ForcePlus(10);
            player.forceGain = true;
            SUIMng.ShowForce();
            Destroy(this.gameObject);
        }
    }
}

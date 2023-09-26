using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthGain : MonoBehaviour
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
            player.iniHp += 20;
            player.healthGain = true;
            float currHpPer = player.currHp / player.iniHp;
            SUIMng.SetHUD(currHpPer);
            SUIMng.ShowHealth();
            Destroy(this.gameObject);
        }
    }
}

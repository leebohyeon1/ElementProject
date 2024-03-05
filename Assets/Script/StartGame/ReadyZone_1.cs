using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyZone_1 : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartManager.instance.readyPlayer1 = true;

            PlayerStats stat = other.GetComponent<PlayerStats>();
            StartManager.instance.playerStats_1 = stat;
            StartManager.instance.playerObject_1 = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartManager.instance.readyPlayer1 = false;
            StartManager.instance.playerStats_1 = null;
            StartManager.instance.playerObject_1 = null;
        }
    }

}

using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyZone_2 : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartManager.instance.readyPlayer2 = true;

            PlayerStats stat = other.GetComponent<PlayerStats>();
            StartManager.instance.playerStats_2 = stat;
            StartManager.instance.playerObject_2 = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartManager.instance.readyPlayer2 = false;
            StartManager.instance.playerStats_2 = null;
            StartManager.instance.playerObject_2 = null;
        }
    }
}

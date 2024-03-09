using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Health : NetworkBehaviour
{
    [Networked(OnChanged = nameof(NetworkedHealthChanged))]
    public float NetworkedHealth { get; set; } = 100;

    private static void NetworkedHealthChanged(Changed<Health> changed)
    {
        // Here you would add code to update the player's healthbar.
        Debug.Log($"Health changed to: {changed.Behaviour.NetworkedHealth}");
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void DealDamageRpc(float damage)
    {
        Debug.Log("데미지 받음");
        NetworkedHealth -= damage;
    }
   
}

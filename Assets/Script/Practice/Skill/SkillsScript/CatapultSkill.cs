using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/ActiveSkill/Catapult")]
public class CatapultSkill : Skill
{
    public int PingPongCount;
    public int Damage;
    public GameObject StonePrefab;

    public override void Activate(GameObject parent)
    {
        PlayerMove playerMove = parent.GetComponent<PlayerMove>();
        NetworkObject Stone = playerMove.Runner.Spawn(StonePrefab);    
    }

}


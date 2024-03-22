using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/ActiveSkill/Fruit")]
public class Fruit : Skill
{

    public int RecoveryAmount;

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);
        PlayerStats stats = parent.GetComponent<PlayerStats>();
        stats.hp += RecoveryAmount;
    }

}
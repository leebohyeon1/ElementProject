using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fruit : Skill
{
  
    public int RecoveryAmount;
    
    void Start()
    {
       
    }

    public override void Activate(GameObject parent)
    {
        base.Activate(parent);
        PlayerStats stats = parent.GetComponent<PlayerStats>();
        Debug.Log(0101010);
        stats.hp += RecoveryAmount;
    }

}

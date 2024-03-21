using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Skill
{
    public string skillName;
    public int skillID;
    public Sprite SkillImage;

    public virtual void Activate(GameObject parent)
    {
        Debug.Log("Skill activated: " + skillName);
    }
}



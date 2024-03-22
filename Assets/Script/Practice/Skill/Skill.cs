using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Skill : ScriptableObject
{
    public string SkillName;
    public int SkillID;
    public float SkillCool;
    public Sprite SkillImage;

    public virtual void Activate(GameObject parent)
    {
        Debug.Log("Skill activated: " + SkillName);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public List<Skill> Skills = new List<Skill>();
    private void Awake()
    {
        instance = this;
    }

    public Skill[] GetRandomSkills(int count)
    {
        Skill[] randomSkills = Skills.ToArray();
        for (int i = 0; i < count; i++)
        {
            randomSkills[i] = Skills[Random.Range(0, Skills.Count)];
        }
        return randomSkills;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveSkill : MonoBehaviour
{
    //public SkillManager Skillmanager;
    public Skill[] playerSkills = new Skill[2];
    public int SkillIndex;

    void Start()
    {
        //Skillmanager = FindObjectOfType<SkillManager>();
        Debug.Log(playerSkills.Length);
        playerSkills = SkillManager.instance.GetRandomSkills(playerSkills.Length +1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && playerSkills[SkillIndex] != null)
        {
            playerSkills[SkillIndex].Activate(gameObject);
        }

        float wheelInput = Input.GetAxis("Mouse ScrollWheel"); 
        if (wheelInput > 0)
        {            
            if (SkillIndex >= 2)
            {
                SkillIndex = 0;
            }
            else
            {
                SkillIndex++;
            }
        }
        else if (wheelInput < 0) 
        {
            if (SkillIndex <= 0)
            {
                SkillIndex = 2;
            }
            else
            {
                SkillIndex--;
            }
        }
    }
}

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

    void Start()
    {      
       // ���⿡ �÷��̾ ������ ��ų���� �ʱ�ȭ�ϴ� �ڵ带 �ۼ��� �� �ֽ��ϴ�.
       Skills.Add(new Fruit {skillName = "ȸ��", skillID = 10, RecoveryAmount = 50 });
       // �߰����� ��ų �ʱ�ȭ �ڵ带 �ۼ��� �� �ֽ��ϴ�.
        
    }

    public Skill[] GetRandomSkills(int count)
    {
        Skill[] randomSkills = new Skill[count];
        for (int i = 0; i < count; i++)
        {
            randomSkills[i] = Skills[Random.Range(0, Skills.Count)];
        }
        return randomSkills;
    }

}

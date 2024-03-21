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
       // 여기에 플레이어가 보유한 스킬들을 초기화하는 코드를 작성할 수 있습니다.
       Skills.Add(new Fruit {skillName = "회복", skillID = 10, RecoveryAmount = 50 });
       // 추가적인 스킬 초기화 코드를 작성할 수 있습니다.
        
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

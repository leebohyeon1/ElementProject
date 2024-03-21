using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArea : NetworkBehaviour
{
    private bool bTrigger = false;

    //public PlayerMove player;
    //public PlayerStats stats;

    //public PlayerMove Player { get => player; set => player = value; }
    //public PlayerStats Stats { get => stats; set => stats = value; }


    private void Awake()
    {
       Destroy(gameObject,0.05f);
    }

    private void OnTriggerEnter(Collider other)
    {
        
        // 충돌한 객체가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 객체의 PlayerMove 스크립트 컴포넌트 가져오기
            PlayerMove playerMove = other.GetComponent<PlayerMove>();
            PlayerStats playerStat = other.GetComponent<PlayerStats>();

            // PlayerMove 스크립트가 존재한다면 TakeDamage 함수 호출        
            if (playerMove != null || !bTrigger)
            {
                playerStat.isHitByOtherInGuard = true;
                if (playerStat.isGuard || this.HasStateAuthority)
                {
                    Debug.Log("가드");
                    bTrigger = true;
                }
                else
                {
                    Debug.Log(1);
                    bTrigger = true;
                    playerMove.TakeDamage();
                }

            }
        }
    }
}

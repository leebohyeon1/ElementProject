using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPre : NetworkBehaviour
{
    private bool bTrigger = false;

    private void Awake()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // 충돌한 객체의 PlayerMove 스크립트 컴포넌트 가져오기
            PlayerController playerController = other.GetComponent<PlayerController>();

            // PlayerMove 스크립트가 존재한다면 TakeDamage 함수 호출
            if (playerController != null || !bTrigger)
            {
                Debug.Log(1);
                bTrigger = true;
                playerController.TakeDamage();
            }
        }
    }

}

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
        
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü�� PlayerMove ��ũ��Ʈ ������Ʈ ��������
            PlayerMove playerMove = other.GetComponent<PlayerMove>();
            PlayerStats playerStat = other.GetComponent<PlayerStats>();

            // PlayerMove ��ũ��Ʈ�� �����Ѵٸ� TakeDamage �Լ� ȣ��        
            if (playerMove != null || !bTrigger)
            {
                playerStat.isHitByOtherInGuard = true;
                if (playerStat.isGuard || this.HasStateAuthority)
                {
                    Debug.Log("����");
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

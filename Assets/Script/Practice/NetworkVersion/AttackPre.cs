using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPre : NetworkBehaviour
{
    private bool bTrigger = false;

    private PlayerMove player;
    public PlayerStats stats;

    public PlayerMove Player { get => player; set => player = value; }
    public PlayerStats Stats { get => stats; set => stats = value; }

    private void Awake()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü�� PlayerMove ��ũ��Ʈ ������Ʈ ��������
            PlayerController playerController = other.GetComponent<PlayerController>();
            PlayerStats playerStat = other.GetComponent<PlayerStats>();
            // PlayerMove ��ũ��Ʈ�� �����Ѵٸ� TakeDamage �Լ� ȣ��
            if (playerController != null || !bTrigger)
            {
                playerStat.isHitByOtherInGuard = true;
                if (playerStat.isGuard)
                {
                    Debug.Log("����");          
                    bTrigger = true;
                }
                else 
                {
                    Debug.Log(1);
                    bTrigger = true;
                    playerController.TakeDamage();
                }
               
            }

        }
    }

}

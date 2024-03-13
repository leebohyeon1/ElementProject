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
        // �浹�� ��ü�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ü�� PlayerMove ��ũ��Ʈ ������Ʈ ��������
            PlayerController playerController = other.GetComponent<PlayerController>();

            // PlayerMove ��ũ��Ʈ�� �����Ѵٸ� TakeDamage �Լ� ȣ��
            if (playerController != null || !bTrigger)
            {
                Debug.Log(1);
                bTrigger = true;
                playerController.TakeDamage();
            }
        }
    }

}

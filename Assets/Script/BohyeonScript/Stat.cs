using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Stat : MonoBehaviour
{

    public static Stat instance;
    public Move move;

    private Rigidbody rb;

    public int Hp;
    public float speed;
    public bool CanControl;
    [Space(3f)]

    [Header("�뽬")]
    public float DashForce;
    public float dashDistance;
    public float dashDuration;
    public float DashCoolTime;
    [Space(3f)]

    [Header("�÷��̾� ����")]
    public bool canControl;
    public bool isDash;
    public bool CanDash;
    public bool isDie;
    [Space(3f)]

    [Header("�� �ν�")]
    public bool isGrounded;
    [Space(3f)]

    [Header("�� �ν�")]
    public bool isTouchingWall;
    public bool isTouchingLeftWall;
    public bool isTouchingRightWall;
    public bool isWallSliding;
    public float wallSlideSpeed = 2f; // ������ �������� �ӵ�
    [Space(3f)]

    [Header("����")]
    public bool isJump;
    public int JumpCount;
    public float jumpForce;
    public bool CanWallJump;
    public bool isWallJump;
    public float wallJumpForce = 5f;
    public float wallJumpTime;
    [Space(3f)]

    [Header("����")]
    public bool isGuard;
    public bool CanGuard;
    public float GuardCool = 5;
    [Space(3f)]

    [Header("����")]
    public bool isSpasticity;
    [Space(3f)]

    [Header("����")]
    public bool isAttack;
    public bool CanAttack;
    public bool isHitByOther; //�ǰݴ��ߴ°�
    [Space(3f)]

    [Header("����")]
    public bool isInvincibility;

    [SerializeField] private Transform playerHead;

    private void Start()
    {
        CanGuard = true;
        CanDash = true;
        CanAttack = true;
        move = GetComponent<Move>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
       
       // ResetJump();
    }
    private void ResetJump()
    {
        if (isGrounded && JumpCount != 2 && !isJump)
        {
            JumpCount = 2;
            //Debug.Log(2);
        }
        else if (isTouchingWall && JumpCount != 1 && !isJump)
        {
            JumpCount = 1;
            //Debug.Log(1);
        }
    }
}

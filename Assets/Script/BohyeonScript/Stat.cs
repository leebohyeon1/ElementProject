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


    public float speed;
   
    [Space(20f)]

    [Header("대쉬")]
    public float DashForce;
    public float dashDistance;
    public float dashDuration;
    public float DashCoolTime;
    [Space(20f)]

    [Header("플레이어 상태")]
    public bool canControl;
    public bool isDash;
    public bool CanDash;
    public bool isDie;
    [Space(3f)]

    [Header("땅 인식")]
    public bool isGrounded;
    [Space(3f)]

    [Header("벽 인식")]
    public bool isTouchingWall;
    public bool isTouchingLeftWall;
    public bool isTouchingRightWall;
    public bool isWallSliding;
    public float wallSlideSpeed = 2f; // 벽에서 내려오는 속도
    [Space(3f)]

    [Header("점프")]
    public bool isJump;
    public int JumpCount;
    public float jumpForce;
    public bool CanWallJump;
    public bool isWallJump;
    public float wallJumpForce = 5f;
    public float wallJumpTime;

    [SerializeField] private Transform playerHead;

    private void Start()
    {
        CanDash = true;
        move = GetComponent<Move>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        isTouchingRightWall = Physics.Raycast(transform.position, transform.right, 0.6f, move.wallLayer);
        isTouchingLeftWall = Physics.Raycast(transform.position, -transform.right, 0.6f, move.wallLayer);
        isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, 0), 0.1f, move.groundLayer);

        if (isWallSliding) //벽 슬라이딩 상태일때 떨어지는 속도 계산
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeed, rb.velocity.z);
        }

        if (isTouchingLeftWall || isTouchingRightWall)
        {
            isTouchingWall = true;
        }
        else
        {
            isTouchingWall = false;
        }

        if (!isGrounded && isTouchingWall && rb.velocity.y < 0 && !isDash) 
        {
            isWallSliding = true; //벽에 붙어있고 아래로 떨어지고 있으면 벽 슬라이딩 상태가 됨(벽 슬라이딩 상태에서만 벽 점프 가능)
        }
        else
        {
            isWallSliding = false;
        }
        ResetJump();
    }
    private void ResetJump()
    {
        if (isGrounded && JumpCount != 2 && !isJump)
        {
            JumpCount = 2;
            Debug.Log(2);
        }
        else if (isTouchingWall && JumpCount != 1 && !isJump)
        {
            JumpCount = 1;
            Debug.Log(1);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;

   

    [Header("이동")]
    public float PlayerSpeed = 2f;
    public float fallMultiplier = 2.5f;
    [Space(3f)]

    [Header("점프")]
    public bool isJump;
    public bool isWallJump;
    public int JumpCount;
    public float JumpForce = 5f;
    public float WallJumpTime = 0.3f;
    public float WallJumpForce = 7;
    [Space(3f)]
    
    [Header("대쉬")]
    public Vector3 DashDirection;
    public bool CanDash;
    public bool isDash;
    public float DashForce;
    public float dashDistance;
    public float dashDuration;
    public float DashCoolTime;
    [Space(3f)]

    [Header("공격")]
    public bool isAttack;
    public bool CanAttack;
    [Space(3f)]

    [Header("지형 인식")]
    public bool isTouchingWall;
    public bool isTouchingRightWall;
    public bool isTouchingLeftWall;
    public bool isGrounded;
    [Space(3f)]

    [Header("벽")]
    public bool isWallSliding;
    public float WallSlideSpeed = 2f;
    public int WallJumpDirection;
    

    void Awake()
    {
        CanDash = true;
        CanAttack = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

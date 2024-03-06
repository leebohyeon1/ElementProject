using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private Stat stats;
    public GameObject Center;

    // #. 컴포넌트 관련 변수
    private Rigidbody rb;

    // #. 이동 관련 기능 
    Vector3 movement;
    private float horizontal;
    private float vertical;
    private Vector3 DashDirection; 


    public float fallMultiplier = 2.5f; //중력 가속도
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    public int WallJumpDirection;
    private float existingWallJumpTime;
   

    private void Start()
    {
        stats = GetComponent<Stat>();
        // Body 게임 오브젝트의 Renderer 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 플레이어 움직임 관련
        InputKey();
        if (!stats.isDash) move();
        RotateAttackArea();

        if (stats.isWallJump) //벽 점프 상태일 때
        {
            stats.wallJumpTime -= Time.deltaTime;
            rb.velocity = new Vector3(WallJumpDirection * stats.wallJumpForce, stats.wallJumpForce, 0);
            if (stats.wallJumpTime <= 0)
            {
                StopWallJump(0);
            }
        }

        if (stats.isTouchingWall && stats.isWallJump && stats.wallJumpTime <= existingWallJumpTime - 0.05f) 
        {
            //벽 점프 상태일때 다른 벽에 붙으면 벽 점프 멈춤
            StopWallJump(0);
        }

        else if (stats.isWallJump && stats.wallJumpTime <= existingWallJumpTime - 0.15f && horizontal != 0)
        {
            //벽 점프 상태일때 움직이면 벽 점프 멈춤
            StopWallJump(1);
        }
    }

    private void InputKey()
    {
        if (Input.GetButtonDown("Dash") && stats.CanDash && !stats.isDash)
        {
            Dash();
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
      
        if (Input.GetKeyDown(KeyCode.Space) && stats.JumpCount != 0 && !stats.isWallSliding)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.Space) && stats.isWallSliding && stats.JumpCount != 0)
        {
            WallJump();
        }

        if (rb.velocity.y < 0) // 플레이어가 아래로 떨어지는 중이면 중력 추가
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            stats.isJump = false; //아래로 떨어지면 점프상태가 풀림
        } 

    }

    private void move()
    {
        //  = new Vector3(horizontal, 0f, vertical) * stats.speed * Time.deltaTime;
        //rb.MovePosition(transform.position + movement);
        if (stats.isTouchingRightWall && horizontal == 1)
        {
            horizontal = 0;
        }
        else if (stats.isTouchingLeftWall && horizontal == -1)
        {
            horizontal = 0;
        }
        movement = new Vector3(horizontal, 0, 0);
        float fallspeed = rb.velocity.y;

        movement.x *= stats.speed;

        movement.y = fallspeed; //벽에 붙었을 때 떨어지는 속도

        rb.velocity = movement;
    }
    #region 점프
    private void Jump()
    {
        stats.isJump = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);
        stats.JumpCount--;
    }

    private void WallJump()
    {      
        existingWallJumpTime = stats.wallJumpTime;

        //벽 점프 상태 온
        stats.isWallJump = true;
        stats.isJump = true;

        rb.velocity = Vector3.zero;

        WallJumpDirection = stats.isTouchingRightWall ? -1 : 1; 

        stats.JumpCount--;
    }

    public void StopWallJump(int Case)
    {
        stats.isWallJump = false;
        switch (Case)
        {
            case 0:
                rb.velocity = Vector3.zero;
                break;
            case 1: 
                rb.velocity /= 2;
                break;
        }

        stats.wallJumpTime = existingWallJumpTime;
    }
    #endregion

    #region 대쉬
    void Dash()
    {
        stats.CanDash = false;
        stats.isDash = true;
        // 대쉬 방향 설정
        Vector3 dashDirection = DashDirection - transform.position;

        // 대쉬 시작

        rb.velocity = dashDirection.normalized * (stats.dashDistance / stats.dashDuration);

        // 대쉬 종료 예약
        Invoke("StopDash", stats.dashDuration);
    }

    void StopDash()
    {
        // 대쉬 끝 후 속도 초기화
        rb.velocity = Vector3.zero;

        // 대쉬 종료
        stats.isDash = false;
        StartCoroutine(ReturnDash());
    }
    public IEnumerator ReturnDash()
    {
        yield return new WaitForSeconds(stats.DashCoolTime);
        stats.CanDash = true;
    }
    #endregion


    private void RotateAttackArea()
    {
        Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
        mPosition.z = oPosition.z - Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
        DashDirection = target;
        float dy = target.y - oPosition.y;
        float dx = target.x - oPosition.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        Center.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
    }

}
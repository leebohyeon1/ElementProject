using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerController : NetworkBehaviour
{
    #region Component
    private Rigidbody rb;
    private PlayerStat Stat;
    #endregion

    public float horizontal;
    private float existingWallJumpTime;
    public GameObject Center;

    public GameObject AttackPrefab;
    public LayerMask groundLayer;
    GameObject aaa;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Stat = GetComponent<PlayerStat>();
    }

    void Update()
    {
        if (Stat.CanControl && HasStateAuthority)
        {
            InputKey();
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }
        if (Stat.CanControl)
        {
            if (!Stat.isDash) move();
            RotateAttackArea();
            BoolSet();
        }
      
        //if (attack != null)
        //attack.transform.localScale = Stat.AttackScale;
    }
     private void InputKey()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && Stat.JumpCount != 0 &&Stat.isWallSliding)
        {         
             WallJump();              
        }
        else if(Input.GetButtonDown("Jump") && Stat.JumpCount != 0)
        {
            Jump();
        }     
        
        if (Input.GetButtonDown("Dash") && Stat.CanDash && !Stat.isDash)

        {
            Dash();

        }

        if (Input.GetMouseButtonDown(0) && Stat.CanAttack)
        {
            Attack();
        }


    }
    private void BoolSet()
    {
        Stat.isTouchingRightWall = Physics.Raycast(transform.position, transform.right, 0.51f, groundLayer);
        Stat.isTouchingLeftWall = Physics.Raycast(transform.position, -transform.right, 0.51f, groundLayer);
        Stat.isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, 0), 0.1f, groundLayer);

        if (Stat.isTouchingLeftWall || Stat.isTouchingRightWall)
        {
            Stat.isTouchingWall = true;
        }
        else
        {
            Stat.isTouchingWall = false;
        }

        if (Stat.isWallSliding)
        {
            rb.velocity = new Vector3(rb.velocity.x, -Stat.WallSlideSpeed, rb.velocity.z);
        }

        //벽에 붙어있고 벽 방향으로 키 입력 시 벽 슬라이딩 상태가 됨(벽 슬라이딩 상태에서만 벽 점프 가능)
        if (/*!stats.isGrounded &&*/ Stat.isTouchingWall && horizontal == -Stat.WallJumpDirection && !Stat.isDash/*&& rb.velocity.y < 0  && !Stat.isAttack */)
        {
            Stat.isWallSliding = true;
        }
        else
        {
            Stat.isWallSliding = false;
        }

        //벽 점프 상태일 때
        if (Stat.isWallJump)
        {
            Stat.WallJumpTime -= Runner.DeltaTime;
            rb.velocity = new Vector3(Stat.WallJumpDirection * Stat.WallJumpForce, Stat.WallJumpForce, 0);
            if (Stat.WallJumpTime <= 0)
            {
                StopWallJump(0);
            }
        }
        if (Stat.isTouchingWall && Stat.isWallJump && Stat.WallJumpTime <= existingWallJumpTime - 0.07f)
        {
            //벽 점프 상태일때 다른 벽에 붙으면 벽 점프 멈춤
            StopWallJump(0);
        }
        else if (Stat.isWallJump && Stat.WallJumpTime <= existingWallJumpTime - 0.1f && horizontal != 0)
        {
            //벽 점프 상태일때 움직이면 벽 점프 멈춤
            StopWallJump(1);
        }

        if(Stat.isDead)
        {
            Stat.CanControl = false;
        }
  
    }
    public void move()
    {
        Vector3 PlayerVelocity = new Vector3(horizontal, 0, 0) * Stat.PlayerSpeed * Runner.DeltaTime;
        float fallspeed = rb.velocity.y;

        PlayerVelocity.y = fallspeed; 
        rb.velocity = PlayerVelocity;

        if (rb.velocity.y < 0) // 플레이어가 아래로 떨어지는 중이면 중력 추가
        {
           rb.velocity += Vector3.up * Physics.gravity.y * (Stat.fallMultiplier - 1) * Runner.DeltaTime;
           Stat.isJump = false; //아래로 떨어지면 점프 상태가 풀림
        }
    }

    #region Jump
    public void Jump()
    {
        Stat.isJump = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * Stat.JumpForce, ForceMode.Impulse);
        Stat.JumpCount--;
    }

    public void WallJump()
    {
        Debug.Log("벽 점프");
        Stat.JumpCount--;

        existingWallJumpTime = Stat.WallJumpTime;


        //벽 점프 상태 온
        Stat.isWallJump = true;
        Stat.isJump = true;

        rb.velocity = Vector3.zero;
    }

    public void StopWallJump(int Case)
    {
        Stat.isWallJump = false;
        switch (Case)
        {
            case 0:
                rb.velocity = Vector3.zero;
                break;
            case 1:
                rb.velocity /= 2;
                break;
        }
        Stat.WallJumpTime = existingWallJumpTime;
    }
    #endregion

    #region Dash
    public void Dash()
    {
        Stat.isWallSliding = false;
        Stat.CanDash = false;
        Stat.isDash = true;

        // 대쉬 방향 설정
        Vector3 dashDirection = Stat.DashDirection - transform.position;

        // 대쉬 시작
        rb.velocity = dashDirection.normalized * (Stat.dashDistance / Stat.dashDuration);

        // 대쉬 종료 예약
        Invoke("StopDash", Stat.dashDuration);
    }

    void StopDash()
    {
        // 대쉬 끝 후 속도 초기화
        rb.velocity = Vector3.zero;

        // 대쉬 종료
        Stat.isDash = false;
        StartCoroutine(ReturnDash());
    }

    public IEnumerator ReturnDash()
    {
        yield return new WaitForSeconds(Stat.DashCoolTime);
        Stat.CanDash = true;
    }
    #endregion

    #region Attack
    NetworkObject attack;
    public void Attack()
    {
        Stat.isAttack = true;
        Stat.CanAttack = false;
        //NetworkObject attackArea = runner.Spawn(AttackAreaPrefab, SimpleAttackPosition.position, SimpleAttackPosition.rotation);
        //GameObject weapon = Instantiate(Weapon, Center.transform);
        //weapon.transform.localScale = Stat.AttackRange;
        aaa =AttackPrefab;
        aaa.transform.localScale = Stat.AttackScale;
        attack = Runner.Spawn(aaa, Center.transform.GetChild(0).transform.position, Center.transform.rotation);
      // attack.transform.localScale = new Vector3(3,1,1);
        Invoke("EndAttack", 0.05f);
    }
    public void EndAttack() //애니메이션이 끝나면 넣으면 됨 
    {
        Stat.isAttack = false;
        Runner.Despawn(attack);
        StartCoroutine(ReturnAttack());
    }
    public IEnumerator ReturnAttack()
    {
        yield return new WaitForSeconds(1);
        Stat.CanAttack = true;
    }
    #endregion

    public void TakeDamage()
    {
        if(Stat.isDead) return;

        Stat.HP --;

        if(Stat.HP <= 0 )
        {
            Stat.isDead = true;
        }
    }
    private void RotateAttackArea()
    {
        Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
        mPosition.z = oPosition.z - Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
        Stat.DashDirection = target;
        float dy = target.y - oPosition.y;
        float dx = target.x - oPosition.x;
        float rotateDegree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        if (!Stat.isAttack)
        {
            Center.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
           
        }
        
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (Stat.isTouchingWall || Stat.isGrounded)
        {
            Stat.WallJumpDirection = Stat.isTouchingRightWall ? -1 : 1;
            Stat.JumpCount = 2;
        }
    }
}

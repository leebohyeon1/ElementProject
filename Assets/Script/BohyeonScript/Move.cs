using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private Stat stats;
    public GameObject Center;
    public GameObject Weapon;
    // #. 컴포넌트 관련 변수
    private Rigidbody rb;

    // #. 이동 관련 기능 
    Vector3 movement;
    private float horizontal;
    private float vertical;
    private Vector3 DashDirection; 


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
        if (stats.CanControl) { InputKey(); }    
        if (!stats.isDash) { move(); }
        BoolSet();
        RotateAttackArea();    
    }

    private void BoolSet()
    {
        //벽 인식
        stats.isTouchingRightWall = Physics.Raycast(transform.position, transform.right, 0.51f, groundLayer); 
        stats.isTouchingLeftWall = Physics.Raycast(transform.position, -transform.right, 0.51f, groundLayer);


        if (stats.isTouchingLeftWall || stats.isTouchingRightWall)
        {
            stats.isTouchingWall = true;
        }
        else
        {
            stats.isTouchingWall = false;
        }

        //땅 인식
        stats.isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, 0), 0.1f, groundLayer);

        //벽 슬라이딩 상태일때 떨어지는 속도 계산
        if (stats.isWallSliding) 
        {
            rb.velocity = new Vector3(rb.velocity.x, -stats.wallSlideSpeed, rb.velocity.z);
        }

       
        //벽에 붙어있고 벽 방향으로 키 입력 시 벽 슬라이딩 상태가 됨(벽 슬라이딩 상태에서만 벽 점프 가능)
        if (/*!stats.isGrounded &&*/ stats.isTouchingWall /*&& rb.velocity.y < 0 */&& !stats.isDash && horizontal == -WallJumpDirection && !stats.isAttack)
        {
            stats.isWallSliding = true; 
        }
        else
        {
            stats.isWallSliding = false;
        }

        //벽 점프 상태일 때
        if (stats.isWallJump) 
        {
            stats.wallJumpTime -= Time.deltaTime;
            rb.velocity = new Vector3(WallJumpDirection * stats.wallJumpForce, stats.wallJumpForce, 0);
            if (stats.wallJumpTime <= 0)
            {
                StopWallJump(0);
            }
        }
        if (stats.isTouchingWall && stats.isWallJump && stats.wallJumpTime <= existingWallJumpTime - 0.07f)
        {
            //벽 점프 상태일때 다른 벽에 붙으면 벽 점프 멈춤
            StopWallJump(0);
        }
        else if (stats.isWallJump && stats.wallJumpTime <= existingWallJumpTime - 0.1f && horizontal != 0)
        {
            //벽 점프 상태일때 움직이면 벽 점프 멈춤
            StopWallJump(1);
        }

        //컨트롤 못하는 상황
        if (stats.isGuard || stats.isSpasticity)
        {
            stats.CanControl = false;
        }
        else
        {
            stats.CanControl = true;
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
      
        //if (Input.GetKeyDown(KeyCode.Space) && stats.JumpCount != 0 && !stats.isWallSliding)
        //{
        //    Jump();
        //}

        if (Input.GetKeyDown(KeyCode.Space) && stats.isWallSliding && stats.JumpCount != 0 )
        {
            WallJump();
        }
        else if(Input.GetKeyDown(KeyCode.Space) && stats.JumpCount != 0)
        {
            Jump();
        } 

        if(Input.GetButtonDown("Guard") && stats.CanGuard)
        {
            StartCoroutine(Guard());
        }

        if( Input.GetMouseButtonDown(0) && stats.CanAttack)
        {
            Attack();
        }
    }

    private void move()
    {
        movement = new Vector3(horizontal, 0, 0);
        float fallspeed = rb.velocity.y;

        movement.x *= stats.speed;

        movement.y = fallspeed; //벽에 붙었을 때 떨어지는 속도

        rb.velocity = movement;


        if (rb.velocity.y < 0) // 플레이어가 아래로 떨어지는 중이면 중력 추가
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (stats.fallMultiplier - 1) * Time.deltaTime;
            stats.isJump = false; //아래로 떨어지면 점프상태가 풀림
        }
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
        stats.JumpCount--;
       
        existingWallJumpTime = stats.wallJumpTime;
        

        //벽 점프 상태 온
        stats.isWallJump = true;
        stats.isJump = true;

        rb.velocity = Vector3.zero;
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
        stats.isWallSliding = false;
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

    #region 가드
    private IEnumerator Guard()
    {
        Debug.Log("가드");
        horizontal = 0;
        Center.transform.parent.GetChild(0).gameObject.SetActive(false);
        stats.CanGuard = false;
        stats.isGuard = true;
        yield return new WaitForSeconds(1);
        stats.isGuard = false;
        Center.transform.parent.GetChild(0).gameObject.SetActive(true);
        if (!stats.isHitByOther)
         Debug.Log("방어... 그러나 아무 일도 없었다.");
        StartCoroutine(ReturnGuard(0));
    }

   private IEnumerator ReturnGuard(int num) 
    {

        switch (num)
        { 
            case 0:
                yield return new WaitForSeconds(stats.GuardCool);
                stats.CanGuard = true;
                break;
            case 1:
                StopCoroutine(ReturnGuard(0));
                yield return new WaitForSeconds(stats.GuardCool /2);         
                stats.CanGuard = true;
                stats.isHitByOther = false;
                break;
        }
       
    }
    #endregion

    public IEnumerator Spasticity()
    {
        Debug.Log("경직");
        horizontal = 0;
        stats.isSpasticity = true;
        yield return new WaitForSeconds(1);
        stats.isSpasticity = false;

    } //경직

    public void Attack()
    {
        stats.isAttack = true;
        stats.CanAttack = false;
        //NetworkObject attackArea = runner.Spawn(AttackAreaPrefab, SimpleAttackPosition.position, SimpleAttackPosition.rotation);
        GameObject weapon = Instantiate(Weapon,Center.transform);
        weapon.transform.localScale = stats.AttackRange;
        Invoke("EndAttack", 0.2f);
    } 
    public void EndAttack() //애니메이션이 끝나면 넣으면 됨 
    {
        stats.isAttack = false;
        
        StartCoroutine(ReturnAttack());
    }
    public IEnumerator ReturnAttack()
    {
        yield return new WaitForSeconds(1);
        stats.CanAttack = true;
    }
    public IEnumerator ReturnInvincibility()
    {
        yield return new WaitForSeconds(2);
        stats.isInvincibility = false;
        stats.isHitByOther  = false ;
    } //무적 리턴

    public void TakeDamage()
    {
        if (!stats.isGuard && !stats.isInvincibility) // 데미지를 받는 상태일 때
        {
            stats.isHitByOther = true;
            stats.Hp--;
            stats.isInvincibility =true;
            StartCoroutine(ReturnInvincibility());
        }
        else if(stats.isGuard) //가드 상태일 때
        {
            Debug.Log("방어 성공");
            stats.isHitByOther = true;
            stats.isGuard = false;
            StartCoroutine(ReturnGuard(1));
        }

        if (stats.Hp >= 1)
        {

        }
        else
        {
            stats.isDie = true;
        }
    }

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
        if (!stats.isAttack)
        {
            Center.transform.rotation = Quaternion.Euler(0f, 0f, rotateDegree);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(stats.isTouchingWall || stats.isGrounded)
        {
            WallJumpDirection = stats.isTouchingRightWall ? -1 : 1;
            stats.JumpCount = 2;
        }
    }
}
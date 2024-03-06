using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HeoWeb.Fusion;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class Move : NetworkBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private stats stats;


    // #. 컴포넌트 관련 변수
    private Rigidbody rb;

    // #. 이동 관련 기능 
    Vector3 movement;
    public GameObject AttackAreaPrefab;
    public Transform SimpleAttackPosition;
    private float horizontal;
    private float vertical;
    private Vector3 DashDirection;
    // #. 공격 관련 변수
    public GameObject Center;

    // #. 색상 관련 변수
    public GameObject Body;
    private Material bodyMat;
    private Color redColor = Color.red;
    private Color grayColor = Color.gray;
    private Color originalColor;

    private void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();
        stats = GetComponent<stats>();

        // Body 게임 오브젝트의 Renderer 컴포넌트 가져오기
        Renderer renderer = Body.GetComponent<Renderer>();
        if (renderer != null) bodyMat = renderer.material;
        originalColor = bodyMat.color;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 플레이어 움직임 관련
        if (this.HasStateAuthority && !(stats.isDie) && stats.canControl)
        {
            InputKey();
            if (!stats.isDash) move();
            RotateAttackArea();
        }

        //if (!stats.CanWallJump && stats.isWallJump)
        //{
        //    rb.velocity = new Vector3(stats.WallJumpDirection * stats.jumpForce / 1.5f, stats.jumpForce, 0);
        //}
    }

    private void InputKey()
    {
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetButtonDown("Jump") && !stats.isDash && stats.JumpCount != 0 && !stats.isWall) Jump();
        if (Input.GetButtonDown("Jump") && !stats.isDash && stats.JumpCount != 0 && stats.isWall) WallJump();
        if (Input.GetButtonDown("Dash") && stats.CanDash && !stats.isDash)
        {
            Dash();
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    private void move()
    {
        //  = new Vector3(horizontal, 0f, vertical) * stats.speed * Time.deltaTime;
        //rb.MovePosition(transform.position + movement);
        if (stats.isWallRight && horizontal == 1)
        {
            horizontal = 0;
        }
        else if (stats.isWallLeft && horizontal == -1)
        {
            horizontal = 0;
        }
        movement = new Vector3(horizontal, 0, 0);
        float fallspeed = rb.velocity.y;

        movement.x *= stats.speed;

        if (!stats.isWallLeft && !stats.isWallRight)
        {
            movement.y = fallspeed;
        }
        else
        {
            movement.y = fallspeed / 1.03f; //벽에 붙었을 때 떨어지는 속도
        }
        rb.velocity = movement;
    }

    //private void Jump()
    //{
    //    rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);
    //}
    #region 점프
    //public void Jump()
    //{
    //    //if (stats.isGround && JumpCount != 2)
    //    //{
    //    //    JumpCount = 2;
    //    //}
    //    //else if (stats.isWallLeft && JumpCount != 1 || stats.isWallRight && JumpCount != 1)
    //    //{
    //    //    JumpCount = 1;
    //    //}
    //    //else if (Input.GetAxisRaw("Horizontal") != 0)
    //    //{
    //    //    stats.isWallJump = false;
    //    //}  
    //    stats.isGround = false;
    //    stats.JumpCount--;
    //    //if (stats.isWallLeft && stats.CanWallJump || stats.isWallRight && stats.CanWallJump)
    //    //{
    //    //    stats.CanWallJump = false;
    //    //    stats.isWallJump = true;
    //    //    stats.isWallLeft = false;
    //    //    stats.isWallRight = false;
    //    //    Debug.Log("벽점프");
    //    //    StartCoroutine(WallJumpStop());
    //    //}
    //    //else
    //    //{
    //    rb.velocity = new Vector3(rb.velocity.x, stats.jumpForce, 0);
    //    Debug.Log("Jump");   
    //}
    void Jump()
    {
        float force = stats.jumpForce;
        if (rb.velocity.y < 0) force -= rb.velocity.y;

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + ((force + 0.5f * Time.fixedDeltaTime * -stats.GravityScale)), 0);
    }
    IEnumerator WallJumpStop()
    {
        yield return new WaitForSeconds(0.4f);
        stats.isWallJump = false;
    }
    public void WallJump()
    {
        rb.velocity = new Vector3(stats.WallJumpDirection * stats.jumpForce, stats.jumpForce, 0);
        // rb.AddForce(new Vector3(stats.WallJumpDirection * stats.jumpForce, stats.jumpForce, 0), ForceMode.Impulse);
        Debug.Log("벽 점프");
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

    private void Attack()
    {
        NetworkObject attackArea = runner.Spawn(AttackAreaPrefab, SimpleAttackPosition.position, SimpleAttackPosition.rotation);
    }

    public void TakeDamage()
    {
        stats.hp--;

        if (stats.hp >= 1)
        {
            StartCoroutine(ChangeColorCoroutine());
        }
        else
        {
            bodyMat.color = grayColor;
            stats.isDie = true;
        }
    }
    private IEnumerator ChangeColorCoroutine()
    {
        Debug.Log("공격 당했습니다.");

        bodyMat.color = redColor;
        yield return new WaitForSeconds(0.2f);
        bodyMat.color = originalColor;
    }

}

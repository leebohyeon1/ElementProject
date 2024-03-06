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


    // #. ������Ʈ ���� ����
    private Rigidbody rb;

    // #. �̵� ���� ��� 
    Vector3 movement;
    public GameObject AttackAreaPrefab;
    public Transform SimpleAttackPosition;
    private float horizontal;
    private float vertical;
    private Vector3 DashDirection;
    // #. ���� ���� ����
    public GameObject Center;

    // #. ���� ���� ����
    public GameObject Body;
    private Material bodyMat;
    private Color redColor = Color.red;
    private Color grayColor = Color.gray;
    private Color originalColor;

    private void Start()
    {
        runner = FindObjectOfType<NetworkRunner>();
        stats = GetComponent<stats>();

        // Body ���� ������Ʈ�� Renderer ������Ʈ ��������
        Renderer renderer = Body.GetComponent<Renderer>();
        if (renderer != null) bodyMat = renderer.material;
        originalColor = bodyMat.color;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // �÷��̾� ������ ����
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
            movement.y = fallspeed / 1.03f; //���� �پ��� �� �������� �ӵ�
        }
        rb.velocity = movement;
    }

    //private void Jump()
    //{
    //    rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);
    //}
    #region ����
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
    //    //    Debug.Log("������");
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
        Debug.Log("�� ����");
    }
    #endregion

    #region �뽬
    void Dash()
    {
        stats.CanDash = false;
        stats.isDash = true;
        // �뽬 ���� ����
        Vector3 dashDirection = DashDirection - transform.position;

        // �뽬 ����
        rb.velocity = dashDirection.normalized * (stats.dashDistance / stats.dashDuration);

        // �뽬 ���� ����
        Invoke("StopDash", stats.dashDuration);
    }

    void StopDash()
    {
        // �뽬 �� �� �ӵ� �ʱ�ȭ
        rb.velocity = Vector3.zero;

        // �뽬 ����
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
        Vector3 mPosition = Input.mousePosition; //���콺 ��ǥ ����
        Vector3 oPosition = transform.position; //���� ������Ʈ ��ǥ ����
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
        Debug.Log("���� ���߽��ϴ�.");

        bodyMat.color = redColor;
        yield return new WaitForSeconds(0.2f);
        bodyMat.color = originalColor;
    }

}

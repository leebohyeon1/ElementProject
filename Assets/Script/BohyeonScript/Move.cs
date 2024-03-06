using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private Stat stats;
    public GameObject Center;

    // #. ������Ʈ ���� ����
    private Rigidbody rb;

    // #. �̵� ���� ��� 
    Vector3 movement;
    private float horizontal;
    private float vertical;
    private Vector3 DashDirection; 


    public float fallMultiplier = 2.5f; //�߷� ���ӵ�
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    public int WallJumpDirection;
    private float existingWallJumpTime;
   

    private void Start()
    {
        stats = GetComponent<Stat>();
        // Body ���� ������Ʈ�� Renderer ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // �÷��̾� ������ ����
        InputKey();
        if (!stats.isDash) move();
        RotateAttackArea();

        if (stats.isWallJump) //�� ���� ������ ��
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
            //�� ���� �����϶� �ٸ� ���� ������ �� ���� ����
            StopWallJump(0);
        }

        else if (stats.isWallJump && stats.wallJumpTime <= existingWallJumpTime - 0.15f && horizontal != 0)
        {
            //�� ���� �����϶� �����̸� �� ���� ����
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

        if (rb.velocity.y < 0) // �÷��̾ �Ʒ��� �������� ���̸� �߷� �߰�
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            stats.isJump = false; //�Ʒ��� �������� �������°� Ǯ��
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

        movement.y = fallspeed; //���� �پ��� �� �������� �ӵ�

        rb.velocity = movement;
    }
    #region ����
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

        //�� ���� ���� ��
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

}
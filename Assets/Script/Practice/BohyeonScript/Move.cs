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
    // #. ������Ʈ ���� ����
    private Rigidbody rb;

    // #. �̵� ���� ��� 
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
        // Body ���� ������Ʈ�� Renderer ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // �÷��̾� ������ ����
        if (stats.CanControl) { InputKey(); }    
        if (!stats.isDash) { move(); }
        BoolSet();
        RotateAttackArea();    
    }

    private void BoolSet()
    {
        //�� �ν�
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

        //�� �ν�
        stats.isGrounded = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.5f, 0), 0.1f, groundLayer);

        //�� �����̵� �����϶� �������� �ӵ� ���
        if (stats.isWallSliding) 
        {
            rb.velocity = new Vector3(rb.velocity.x, -stats.wallSlideSpeed, rb.velocity.z);
        }

       
        //���� �پ��ְ� �� �������� Ű �Է� �� �� �����̵� ���°� ��(�� �����̵� ���¿����� �� ���� ����)
        if (/*!stats.isGrounded &&*/ stats.isTouchingWall /*&& rb.velocity.y < 0 */&& !stats.isDash && horizontal == -WallJumpDirection && !stats.isAttack)
        {
            stats.isWallSliding = true; 
        }
        else
        {
            stats.isWallSliding = false;
        }

        //�� ���� ������ ��
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
            //�� ���� �����϶� �ٸ� ���� ������ �� ���� ����
            StopWallJump(0);
        }
        else if (stats.isWallJump && stats.wallJumpTime <= existingWallJumpTime - 0.1f && horizontal != 0)
        {
            //�� ���� �����϶� �����̸� �� ���� ����
            StopWallJump(1);
        }

        //��Ʈ�� ���ϴ� ��Ȳ
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

        movement.y = fallspeed; //���� �پ��� �� �������� �ӵ�

        rb.velocity = movement;


        if (rb.velocity.y < 0) // �÷��̾ �Ʒ��� �������� ���̸� �߷� �߰�
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (stats.fallMultiplier - 1) * Time.deltaTime;
            stats.isJump = false; //�Ʒ��� �������� �������°� Ǯ��
        }
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
        stats.JumpCount--;
       
        existingWallJumpTime = stats.wallJumpTime;
        

        //�� ���� ���� ��
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

    #region �뽬
    void Dash()
    {
        stats.isWallSliding = false;
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

    #region ����
    private IEnumerator Guard()
    {
        Debug.Log("����");
        horizontal = 0;
        Center.transform.parent.GetChild(0).gameObject.SetActive(false);
        stats.CanGuard = false;
        stats.isGuard = true;
        yield return new WaitForSeconds(1);
        stats.isGuard = false;
        Center.transform.parent.GetChild(0).gameObject.SetActive(true);
        if (!stats.isHitByOther)
         Debug.Log("���... �׷��� �ƹ� �ϵ� ������.");
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
        Debug.Log("����");
        horizontal = 0;
        stats.isSpasticity = true;
        yield return new WaitForSeconds(1);
        stats.isSpasticity = false;

    } //����

    public void Attack()
    {
        stats.isAttack = true;
        stats.CanAttack = false;
        //NetworkObject attackArea = runner.Spawn(AttackAreaPrefab, SimpleAttackPosition.position, SimpleAttackPosition.rotation);
        GameObject weapon = Instantiate(Weapon,Center.transform);
        weapon.transform.localScale = stats.AttackRange;
        Invoke("EndAttack", 0.2f);
    } 
    public void EndAttack() //�ִϸ��̼��� ������ ������ �� 
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
    } //���� ����

    public void TakeDamage()
    {
        if (!stats.isGuard && !stats.isInvincibility) // �������� �޴� ������ ��
        {
            stats.isHitByOther = true;
            stats.Hp--;
            stats.isInvincibility =true;
            StartCoroutine(ReturnInvincibility());
        }
        else if(stats.isGuard) //���� ������ ��
        {
            Debug.Log("��� ����");
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
        Vector3 mPosition = Input.mousePosition; //���콺 ��ǥ ����
        Vector3 oPosition = transform.position; //���� ������Ʈ ��ǥ ����
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
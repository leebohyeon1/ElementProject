using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using HeoWeb.Fusion;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] private NetworkRunner runner;
    [SerializeField] private PlayerStats stats;

  
    // #. 컴포넌트 관련 변수
    private Rigidbody rb;

    // #. 이동 관련 기능 
    public GameObject AttackAreaPrefab;
    public Transform SimpleAttackPosition;
    private float horizontal;
    private float vertical;

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
        stats = GetComponent<PlayerStats>();

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

            Move();
            RotateAttackArea();
        }
    }

    private void InputKey()
    {
        if (Input.GetMouseButtonDown(0)) Attack();
        if (Input.GetButtonDown("Jump")) Jump();
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        Vector3 movement = new Vector3(horizontal, 0f, vertical) * stats.speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * stats.jumpForce, ForceMode.Impulse);
    }

    private void RotateAttackArea()
    {
        Vector3 mPosition = Input.mousePosition; //마우스 좌표 저장
        Vector3 oPosition = transform.position; //게임 오브젝트 좌표 저장
        mPosition.z = oPosition.z - Camera.main.transform.position.z;
        Vector3 target = Camera.main.ScreenToWorldPoint(mPosition);
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

        if(stats.hp >= 1)
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

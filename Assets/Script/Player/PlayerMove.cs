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

  
    // #. ������Ʈ ���� ����
    private Rigidbody rb;

    // #. �̵� ���� ��� 
    public GameObject AttackAreaPrefab;
    public Transform SimpleAttackPosition;
    private float horizontal;
    private float vertical;

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
        stats = GetComponent<PlayerStats>();

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
        Vector3 mPosition = Input.mousePosition; //���콺 ��ǥ ����
        Vector3 oPosition = transform.position; //���� ������Ʈ ��ǥ ����
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
        Debug.Log("���� ���߽��ϴ�.");

        bodyMat.color = redColor;
        yield return new WaitForSeconds(0.2f);
        bodyMat.color = originalColor;
    }

}

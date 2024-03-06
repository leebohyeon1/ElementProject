using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using HeoWeb.Fusion;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

// #. �÷��̾� ���� ������
public class stats : NetworkBehaviour
{
    [Networked(OnChanged = nameof(UpdatePlayerName))] public NetworkString<_32> PlayerName { get; set; }// ���ڿ��� ���̸� 32�ڷ� ����
    [Networked(OnChanged = nameof(UpdateHat))] public int hatIndex { get; set; }// ���ڿ��� ���̸� 32�ڷ� ����

    [SerializeField] TMP_Text playerNameLabel;

    public static stats instance;

    private GameObject currentHat = null;


    public int hp;
    public float speed;
    public float jumpForce;
    public int JumpCount;
    public int GravityScale;
    [Space(20f)]

    [Header("�뽬")]
    public float DashForce;
    public float dashDistance;
    public float dashDuration;
    public float DashCoolTime;
    [Space(20f)]

    [Header("�÷��̾� ����")]
    public bool canControl;
    public bool CanWallJump;
    public bool isWallJump;
    public bool isDash;
    public bool CanDash;
    public bool isDie;
    [Space(3f)]
    [Header("�� �ν�")]
    public bool isGround;
    [Space(3f)]

    [Header("�� �ν�")]
    public bool isWallLeft; //���� ���ʿ� ���� ���
    public bool isWallRight; //���� �����ʿ� ���� ���
    public bool isWall;
    public int WallJumpDirection;



    [SerializeField] private Transform playerHead;

    private void Start()
    {
        CanDash = true;
        if (this.HasStateAuthority)
        {
            PlayerName = FusionConnection.instance._playerNmae;
            if (instance == null) { instance = this; }

            hp = 2;
        }
    }
    private void Update()
    {
        if (isWallLeft || isWallRight)
        {
            isWall = true;
        }
        else
        {
            isWall = false;
        }
    }

    protected static void UpdatePlayerName(Changed<stats> changed)
    {
        changed.Behaviour.playerNameLabel.text = changed.Behaviour.PlayerName.ToString();
    }

    protected static void UpdateHat(Changed<stats> changed)
    {
        int _hatIndex = changed.Behaviour.hatIndex;
        GameObject _currentHat = changed.Behaviour.currentHat;

        GameObject hat = Hats.hats[_hatIndex];

        if (hat != null)
        {
            Destroy(_currentHat);
        }

        GameObject newHat = GameObject.Instantiate(hat);
        newHat.transform.parent = changed.Behaviour.playerHead;
        newHat.transform.localPosition = Vector3.zero;
        newHat.transform.localEulerAngles = Vector3.zero;
        newHat.GetComponent<Collider>().enabled = false;

        changed.Behaviour.currentHat = newHat;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGround = true;
            JumpCount = 2;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall") && !isGround)
        {
            if (other.transform.position.x < transform.position.x)
            {
                isWallLeft = true;
                WallJumpDirection = 1;
            }
            else if (other.transform.position.x > transform.position.x)
            {
                isWallRight = true;
                WallJumpDirection = -1;
            }
            JumpCount = 1;
            CanWallJump = true;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGround = false;
        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {

            if (other.transform.position.x < transform.position.x)
            {
                isWallLeft = false;
            }
            else if (other.transform.position.x > transform.position.x)
            {
                isWallRight = false;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using HeoWeb.Fusion;
using System;

// #. 플레이어 정보 데이터
public class PlayerStats : NetworkBehaviour
{
    [Networked(OnChanged = nameof(UpdatePlayerName))] public NetworkString<_32> PlayerName { get; set; }// 문자열의 길이를 32자로 제한
    [Networked(OnChanged = nameof(UpdateHat))] public int hatIndex { get; set; }// 문자열의 길이를 32자로 제한

    [SerializeField] TMP_Text playerNameLabel;

    public static PlayerStats instance;

    private GameObject currentHat = null;


    public int hp;



    [Header("플레이어 상태")]
    public bool CanControl;
    public bool isDead;
    [Space(3f)]

    [Header("이동")]
    public float PlayerSpeed;
    public float fallMultiplier = 2.5f;
    [Space(3f)]

    [Header("점프")]
    public bool isJump;
    public bool isWallJump;
    public int JumpCount;
    public float JumpForce;
    public float WallJumpTime = 0.3f;
    public float WallJumpForce = 7;
    [Space(3f)]

    [Header("대쉬")]
    public Vector3 DashDirection;
    public bool CanDash;
    public bool isDash;
    public float DashForce;
    public float dashDistance;
    public float dashDuration;
    public float DashCoolTime;
    [Space(3f)]

    [Header("공격")]
    public bool isAttack;
    public bool CanAttack;
    //[Networked]
    //public Vector3 AttackScale { get; set; }
    [Space(3f)]

    [Header("스킬")]
    public int[] HaveSkill_ID;
    [Space(3f)]
    
    [Header("가드")]
    public bool isGuard;
    public bool CanGuard;
    public float GuardCool;
    public bool isHitByOtherInGuard;
    [Space(3f)]

    [Header("경직")]
    public bool isSpasticity;


    [Header("지형 인식")]
    public bool isTouchingWall;
    public bool isTouchingRightWall;
    public bool isTouchingLeftWall;
    public bool isGrounded;
    [Space(3f)]

    [Header("벽")]
    public bool isWallSliding;
    public float WallSlideSpeed = 2f;
    public int WallJumpDirection;

    [SerializeField] private Transform playerHead;

    private void Start()
    {
        if(this.HasStateAuthority)
        {
            CanGuard = true;
            CanControl = true;
            CanDash = true;
            CanAttack = true;

            PlayerName = FusionConnection.instance._playerNmae;
            if (instance == null) { instance = this; }

            hp = 2;
        }
    }

    protected static void UpdatePlayerName(Changed<PlayerStats> changed)
    {
        changed.Behaviour.playerNameLabel.text = changed.Behaviour.PlayerName.ToString();
    }

    protected static void UpdateHat(Changed<PlayerStats> changed)
    {
        int _hatIndex = changed.Behaviour.hatIndex;
        GameObject _currentHat = changed.Behaviour.currentHat;

        GameObject hat = Hats.hats[_hatIndex];

        if(hat != null)
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

}
    
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;
using HeoWeb.Fusion;
using System;

// #. �÷��̾� ���� ������
public class PlayerStats : NetworkBehaviour
{
    [Networked(OnChanged = nameof(UpdatePlayerName))] public NetworkString<_32> PlayerName { get; set; }// ���ڿ��� ���̸� 32�ڷ� ����
    [Networked(OnChanged = nameof(UpdateHat))] public int hatIndex { get; set; }// ���ڿ��� ���̸� 32�ڷ� ����

    [SerializeField] TMP_Text playerNameLabel;

    public static PlayerStats instance;

    private GameObject currentHat = null;


    public int hp;
    public float speed;
    public float jumpForce;

    [Header("�÷��̾� ����")]
    public bool canControl;
    public bool isDie;


    [SerializeField] private Transform playerHead;

    private void Start()
    {
        if(this.HasStateAuthority)
        {
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
    
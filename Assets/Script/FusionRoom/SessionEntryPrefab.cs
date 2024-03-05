using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HeoWeb.Fusion;


public class SessionEntryPrefab : MonoBehaviour
{
    public TextMeshProUGUI sesisonName;
    public TextMeshProUGUI playerCount;
    public Button joinButton;

    private void Awake()
    {
        joinButton.onClick.AddListener(JoinSession);
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }

    private void JoinSession()
    {
        FusionConnection.instance.ConnectToSeeeion(sesisonName.text);
    }

}

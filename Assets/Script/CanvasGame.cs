using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using HeoWeb.Fusion;
using TMPro;


public class CanvasGame : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    public static CanvasGame instance;

    public TMP_Text text_socrePlayer1;
    public TMP_Text text_socrePlayer2;

    public GameObject panel_Player1_Win;
    public GameObject panel_Player2_Win;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            runner = FindObjectOfType<NetworkRunner>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void ScoreBoardUpdate()
    {
        text_socrePlayer1.text = FusionConnection.instance.socrePlayer_1.ToString();
        text_socrePlayer2.text = FusionConnection.instance.socrePlayer_2.ToString();
    }

    public void Resultupdate(int winnerPlayerIndex)
    {
        if (winnerPlayerIndex == 1)
        {
            panel_Player1_Win.SetActive(true);
        }
        else if (winnerPlayerIndex == 2)
        {
            panel_Player2_Win.SetActive(true);
        }
       
    }
}

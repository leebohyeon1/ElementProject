using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using HeoWeb.Fusion;

enum PlayType
{
    PLAY, // 라운드 진행 중
    LOADING, // 라운드 종료 후 정보 처리
    END   // 게임 종료
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner runner;
    public static GameManager Instance { get; private set; }

    [Header("게임 정보")]
    private PlayType playType;

    [Header("플레이어 정보")]
    public GameObject playerObject_1;
    public GameObject playerObject_2;
    public PlayerStats playerStats_1;
    public PlayerStats playerStats_2;


    [Header("스테이지 정보")]
    private int stageNum;
    public GameObject[] Stages;
    public NetworkObject nowStageNetwork;
    public Transform spawnPoint_1;
    public Transform spawnPoint_2;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            runner = FindObjectOfType<NetworkRunner>();

            playType = PlayType.LOADING;

            SetStage();
            SetPlayerReset();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }


    private void Update()
    {
        if (playType == PlayType.PLAY)
        {
            if (playerStats_1.isDead || playerStats_2.isDead)
            {
                playType = PlayType.LOADING;

                SetStage();
                SetPlayerReset();
            }
        }

    }



    private void SetStage()
    {
        StartCoroutine(SetStage__());
    }
    IEnumerator SetStage__()
    {
        if (nowStageNetwork != null)
        {
            runner.Despawn(nowStageNetwork);
        }

        yield return new WaitForSeconds(1.5f);


        int randomIndex = Random.Range(0, Stages.Length);

        NetworkObject stagePrefab = runner.Spawn(Stages[randomIndex], Vector3.zero);
        nowStageNetwork = stagePrefab;

        spawnPoint_1 = stagePrefab.gameObject.GetComponent<Stage>().SpawnPoint_1;
        spawnPoint_2 = stagePrefab.gameObject.GetComponent<Stage>().SpawnPoint_2;

        yield return new WaitForSeconds(1.0f);

        NetworkObject playerObject1 = runner.GetPlayerObject(0);
        playerObject1.transform.position = spawnPoint_1.position;
        playerObject1.transform.rotation = spawnPoint_1.rotation;

        // 1번 플레이어의 위치와 회전 설정
        NetworkObject playerObject2 = runner.GetPlayerObject(1);
        playerObject2.transform.position = spawnPoint_2.position;
        playerObject2.transform.rotation = spawnPoint_2.rotation;
    }



    private void SetPlayerReset()
    {
        StartCoroutine(SetPlayerReset__());
    }
    IEnumerator SetPlayerReset__()
    {
        playerStats_1.CanControl = false;
        playerStats_2.CanControl = false;

        yield return new WaitForSeconds(2f);

        playerStats_1.hp = 2;
        playerStats_1.isDead = false;
      
        playerStats_2.hp = 2;
        playerStats_2.isDead = false;

        yield return new WaitForSeconds(1f);

        playerStats_1.CanControl = true;
        playerStats_2.CanControl = true;

        yield return new WaitForSeconds(0.6f);

        playType = PlayType.PLAY;
    }



}

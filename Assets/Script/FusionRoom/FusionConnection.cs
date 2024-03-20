using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

namespace HeoWeb.Fusion
{
    public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
    {

        // 룸리스트 서버 관련
        #region

        public static FusionConnection instance;
        public bool connectOnAwake = false;
        [HideInInspector] public NetworkRunner runner;

        [SerializeField] NetworkObject playerPrefab;

        public string _playerNmae = null;

        [Header("Session List")]
        public GameObject roomListCanvas; 
        private List<SessionInfo> _sessions = new List<SessionInfo>();
        public Button refreshButton;
        public Transform sessionListContent;
        public GameObject sessionEntryPrefab;

        public GameObject startManager;

        private void Awake()
        {
            if(instance == null) { instance = this;}
        }

        public void ConnectToLobby(string playerName)
        {
            roomListCanvas.SetActive(true);

            _playerNmae = playerName;

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            runner.JoinSessionLobby(SessionLobby.Shared);
        }

        public async void ConnectToSeeeion(string sessionName)
        {
            roomListCanvas.SetActive(false);

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
                PlayerCount = 2,
            });
        }

        public async void CreateToSeesion()
        {
            roomListCanvas.SetActive(false);

            int randomInt = UnityEngine.Random.Range(1000,9999);
            string randomSessionName = "Room-" + randomInt.ToString();

            if (runner == null)
            {
                runner = gameObject.AddComponent<NetworkRunner>();
            }

            await runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Shared,
                SessionName = randomSessionName,
                PlayerCount = 2,
            });

        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
            NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);

            Debug.Log("스타트 매니저 생성");
            NetworkObject startManagerObject = runner.Spawn(startManager, Vector3.zero);

            runner.SetPlayerObject(runner.LocalPlayer, playerObject);
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {

        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {

        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {

        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {

        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {

        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {

        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {

        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerJoined");
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {

        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {

        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {

        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {

        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            _sessions.Clear();
            _sessions = sessionList;
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {

        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {

        }

        public void RefreshSessionListUI()
        {
            // clear out Session List UI so we don't create duplicates
            foreach (Transform child in sessionListContent)
            {
                Destroy(child.gameObject);
            }

            foreach (SessionInfo session in _sessions)
            {
                if (session.IsVisible)
                {
                    GameObject entry = GameObject.Instantiate(sessionEntryPrefab, sessionListContent);
                    SessionEntryPrefab script = entry.GetComponent<SessionEntryPrefab>();
                    script.sesisonName.text = session.Name;
                    script.playerCount.text = session.PlayerCount + "/" + session.MaxPlayers;

                    if (session.IsOpen == false || session.PlayerCount >= session.MaxPlayers)
                    {
                        script.joinButton.interactable = false;
                    }
                    else
                    {
                        script.joinButton.interactable = true;
                    }
                }
            }
        }

        #endregion



        // 플레이 관련
        #region

        enum PlayMode
        {
            Lobby, // 로비
            PLAY, // 라운드 진행 중
            LOADING, // 라운드 종료 후 정보 처리
            END,   // 게임 종료
            TURN  // 한숨 돌리는 용
        }

        private PlayMode playMode;
        public void PlayModeSetting(int playModeNum)
        {
            if(playModeNum == 1) playMode = PlayMode.PLAY;
            if(playModeNum == 2) playMode = PlayMode.LOADING;
            if(playModeNum == 3) playMode = PlayMode.END;
        }


        [Networked] public int socrePlayer_1 { get; set; }
        [Networked] public int socrePlayer_2 { get; set; }


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

        [Header("관리 오브젝트")]
        public GameObject canvasGame;
        private CanvasGame canvasGameScript;


        private void Update()
        {
            if (playMode == PlayMode.PLAY)
            {
                if (playerStats_1.isDead || playerStats_2.isDead)
                {
                    playMode = PlayMode.LOADING;

                    SetStage();
                    
                }
            }
            else if(playMode == PlayMode.LOADING)
            {
                playMode = PlayMode.TURN;

                if (playerStats_1.isDead) socrePlayer_2++;
                if (playerStats_2.isDead) socrePlayer_1++;
                canvasGameScript.ScoreBoardUpdate();

                if(socrePlayer_1 >= 3 || socrePlayer_2 >= 3)
                {
                    playMode = PlayMode.END;
                }
                else
                {
                    SetPlayerReset();
                }
            }
            else if(playMode == PlayMode.END)
            {
                if(socrePlayer_1 >= 3)
                {
                    canvasGameScript.Resultupdate(1);
                }  
                else if(socrePlayer_2 >= 3)
                {
                    canvasGameScript.Resultupdate(2);
                }
            }
            
        }



        public void FirstStart()
        {
            socrePlayer_1 = 0;
            socrePlayer_2 = 0;

            PlayModeSetting(1);
            SetStage();


            NetworkObject gameCan = runner.Spawn(canvasGame, Vector3.zero);
            canvasGameScript = gameCan.GetComponent<CanvasGame>();
            canvasGameScript.ScoreBoardUpdate();
        }



        public void SetStage()
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

            int randomIndex = UnityEngine.Random.Range(0, Stages.Length);


            NetworkObject playerObject1 = runner.GetPlayerObject(0);
            Rigidbody playerRigidbody1 = playerObject1.GetComponent<Rigidbody>();
            playerRigidbody1.velocity = Vector3.zero;
            playerRigidbody1.angularVelocity = Vector3.zero;
            playerRigidbody1.isKinematic = true;

            NetworkObject playerObject2 = runner.GetPlayerObject(1);
            Rigidbody playerRigidbody2 = playerObject2.GetComponent<Rigidbody>();
            playerRigidbody2.velocity = Vector3.zero;
            playerRigidbody2.angularVelocity = Vector3.zero;
            playerRigidbody2.isKinematic = true;

            if (runner.IsSharedModeMasterClient)
            {
                NetworkObject stagePrefab = runner.Spawn(Stages[randomIndex], Vector3.zero);
                nowStageNetwork = stagePrefab; 

                spawnPoint_1 = stagePrefab.gameObject.GetComponent<Stage>().SpawnPoint_1;
                spawnPoint_2 = stagePrefab.gameObject.GetComponent<Stage>().SpawnPoint_2;

                yield return new WaitForSeconds(0.5f); 

                // 1번 플레이어 위치 설정
                playerObject1.transform.position = spawnPoint_1.position;
                playerObject1.transform.rotation = spawnPoint_1.rotation;
                playerRigidbody1.isKinematic = false;

                // 2번 플레이어 위치 설정
                playerObject2.transform.position = spawnPoint_2.position;
                playerObject2.transform.rotation = spawnPoint_2.rotation;
                playerRigidbody2.isKinematic = false;
            }
            else
            {
                GameObject stagePrefab = Stages[randomIndex];

                spawnPoint_1 = stagePrefab.gameObject.GetComponent<Stage>().SpawnPoint_1;
                spawnPoint_2 = stagePrefab.gameObject.GetComponent<Stage>().SpawnPoint_2;

                yield return new WaitForSeconds(0.5f);

                // 1번 플레이어 위치 설정
                playerObject1.transform.position = spawnPoint_1.position;
                playerObject1.transform.rotation = spawnPoint_1.rotation;
                playerRigidbody1.isKinematic = false;

                // 2번 플레이어 위치 설정
                playerObject2.transform.position = spawnPoint_2.position;
                playerObject2.transform.rotation = spawnPoint_2.rotation;
                playerRigidbody2.isKinematic = false;
            }
        }



        public void SetPlayerReset()
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

            playMode = PlayMode.PLAY;
        }







        #endregion

    }




}


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Smashball.Input;
using Smashball.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Smashball.Gameplay
{
    public sealed class RoundManager : MonoBehaviour, IRoundService
    {
        [SerializeField] private Color opponentColor;
        [SerializeField] private Color playerColor;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private float serveMeterSpeed = 1.5f;
        [SerializeField] private int scoreToWin = 3;


        public Color PlayerColor => playerColor;
        public Color OpponentColor => opponentColor;
        public int StartingPlayer { get; private set; }
        public float ServeQuality { get; private set; }
        public BallController CurrentBall { get; private set; }
        public RoundState State { get; private set; }

        private ICameraShake cameraShake;
        private IArenaBounds arenaBounds;
        private IInputService inputService;
        private UIManager uiManager;
        private List<PlayerController> players = new();
        private float serveMeterTimer;
        private int playerScore;
        private int opponentScore;
        private CameraPoseController camPose;
        private CameraFollow camFollow;
        
        private void Awake()
        {
            Services.Register<IRoundService>(this);
        }

        private void Start()
        {
            inputService = Services.Get<IInputService>();
            uiManager = Services.Get<UIManager>();
            arenaBounds = Services.Get<IArenaBounds>();
            cameraShake = Services.Get<ICameraShake>();
            uiManager.ShowJoystick(false);
            var cam = Camera.main;
            camPose = cam.transform.parent.GetComponent<CameraPoseController>();
            camFollow = cam.transform.parent.GetComponent<CameraFollow>();
        }

        private void Update()
        {
            if (State != RoundState.Serving)
                return;

            serveMeterTimer += Time.deltaTime * serveMeterSpeed;
            ServeQuality = Mathf.PingPong(serveMeterTimer, 1f);
        }

        public void StartGame()
        {
            StartRound();
        }

        public bool IsStartingPlayer(PlayerController playerController)
        {
            return players[StartingPlayer] == playerController;
        }

        private void ResetServeMeter()
        {
            serveMeterTimer = 0f;
            ServeQuality = 0f;
        }

        public PlayerController GetOtherPlayer(PlayerController player)
        {
            if (player == players[0])
                return players[1];
            return players[0];
        }

        private void StartRound()
        {
            playerScore = 0;
            opponentScore = 0;
            uiManager.UpdateScore(playerScore, opponentScore);

            if (CurrentBall == null)
            {
                CurrentBall = Instantiate(ballPrefab).GetComponent<BallController>();
                CurrentBall.Init(arenaBounds);                
            }
            CurrentBall.gameObject.SetActive(false);

            foreach (var player in players)
            {
                Destroy(player.gameObject);
            }

            players.Clear();

            AddPlayer(true, true);
            AddPlayer(false, false);
            var startingPlayer = Random.Range(0, 2);
            DoServe(startingPlayer);
            
            camFollow.SetTarget(players[1].transform);
        }

        private void AddPlayer(bool isTopPlayer, bool isBot)
        {
            var go = Instantiate(playerPrefab);
            var pc = go.GetComponent<PlayerController>();

            if (isBot)
            {
                var bot = go.AddComponent<BotController>();
                bot.Init();
                pc.Init(isTopPlayer, bot.Input, this, arenaBounds, cameraShake);
            }
            else
            {
                var humanInput = new HumanPlayerInput(inputService);
                pc.Init(isTopPlayer, humanInput, this, arenaBounds, cameraShake);
            }

            players.Add(pc);
        }

        public void OnServed()
        {
            uiManager.ServeUI.Show(false);
            uiManager.ShowJoystick(true);
            State = RoundState.Playing;
            _ = camPose.ToGameplayAsync();
            camFollow.SetEnabled(true);
        }

        public async Task OnPlayerHitByBall(PlayerController player)
        {
            uiManager.ShowJoystick(false);
            var playerHitOpponent = player == players[0];
            if (playerHitOpponent)
                playerScore++;
            else
                opponentScore++;
            uiManager.UpdateScore(playerScore, opponentScore);
            State = RoundState.Smashed;
            CurrentBall.gameObject.SetActive(false);
            await uiManager.ShowSmashedFeedback(player, playerHitOpponent);
            if (opponentScore >= scoreToWin || playerScore >= scoreToWin)
            {
                State = RoundState.GameOver;
                uiManager.ShowGameOverUI(playerScore >= scoreToWin);
            }
            else
            {
                DoServe(playerHitOpponent ? 0 : 1);
            }
        }

        private void DoServe(int startingPlayer)
        {
            foreach (var player in players)
            {
                player.ResetPosition();
            }
            StartingPlayer = startingPlayer;
            State = RoundState.Serving;
            ResetServeMeter();
            var playerServing = StartingPlayer != 0;
            uiManager.ServeUI.Show(playerServing);
            camFollow.SetEnabled(!playerServing);
            if (playerServing) _ = camPose.ToServeAsync();
        }
    }
}
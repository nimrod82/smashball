using System.Collections.Generic;
using Smashball.Input;
using Smashball.UI;
using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class RoundManager : MonoBehaviour, IRoundService
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject ballPrefab;
        [SerializeField] private float serveMeterSpeed = 1.5f;

        public int StartingPlayer { get; private set; }
        public float ServeQuality { get; private set; } 
        public BallController CurrentBall { get; private set; }
        public RoundState State { get; private set; }
        
        private IInputService inputService;
        private UIManager uiManager;
        private List<PlayerController> players = new();
        private float serveMeterTimer;

        private void Awake()
        {
            Services.Register<IRoundService>(this);
        }

        private void Start()
        {
            inputService = Services.Get<IInputService>();
            uiManager = Services.Get<UIManager>();
            uiManager.ShowJoystick(false);
            StartRound();
        }
        
        private void Update()
        {
            if (State != RoundState.Serving)
                return;

            serveMeterTimer += Time.deltaTime * serveMeterSpeed;
            ServeQuality = Mathf.PingPong(serveMeterTimer, 1f);
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
            if(player == players[0])
                return players[1];
            return players[0];
        }

        private void StartRound()
        {
            if (CurrentBall == null)
                CurrentBall = Instantiate(ballPrefab).GetComponent<BallController>();
            
            foreach (var player in players)
            {
                Destroy(player.gameObject);
            }
            players.Clear();

            AddPlayer(true, true);
            AddPlayer(false, false);
            var startingPlayer = Random.Range(0, 2);
            DoServe(startingPlayer);
        }

        private void AddPlayer(bool isTopPlayer, bool isBot)
        {
            var go = Instantiate(playerPrefab);
            var pc = go.GetComponent<PlayerController>();

            if (isBot)
            {
                var bot = go.AddComponent<BotController>();
                bot.Init();
                pc.Init(isTopPlayer, bot.Input);
            }
            else
            {
                var humanInput = new HumanPlayerInput(inputService);
                pc.Init(isTopPlayer, humanInput);
            }

            players.Add(pc);
        }

        public void OnServed()
        {
            uiManager.ServeUI.Hide();
            uiManager.ShowJoystick(true);
            State = RoundState.Playing;
        }
        
        public void OnPlayerHitByBall(PlayerController player)
        {
        }

        private void DoServe(int startingPlayer)
        {
            StartingPlayer = 0;// startingPlayer;
            State = RoundState.Serving;
            ResetServeMeter();

            if (StartingPlayer != 0)
                uiManager.ServeUI.Show();
            
            CurrentBall.gameObject.SetActive(false);
        }
    }
}
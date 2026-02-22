using System.Collections.Generic;
using Smashball.Input;
using UnityEngine;

namespace Smashball.Gameplay
{
    public sealed class RoundManager : MonoBehaviour, IRoundService
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject ballPrefab;
       
        public BallController CurrentBall { get; private set; }
        
        private IInputService inputService;
        private List<PlayerController> players = new();

        private void Awake()
        {
            Services.Register<IRoundService>(this);
        }

        private void Start()
        {
            inputService = Services.Get<IInputService>();
            StartRound();
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
        
        public void OnPlayerHitByBall(PlayerController player)
        {
        }

        private void DoServe(int startingPlayer)
        {
            CurrentBall.gameObject.SetActive(false);
            players[startingPlayer].StartServing();
        }
    }
}
using System.Threading.Tasks;
using Smashball.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Smashball.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] ServeUI serveUI;
        [SerializeField] CanvasGroup joystickCanvasGroup;
        [SerializeField] GameObject mainMenuCanvas;
        [SerializeField] GameObject inGameUICanvas;
        [SerializeField] GameOverUI gameOverUI;
        [SerializeField] Button playButton;
        [SerializeField] TextMeshProUGUI playerScoreText;
        [SerializeField] TextMeshProUGUI opponentScoreText;
        [SerializeField] SmashedFeedbackUI smashFeedback;
        
        public ServeUI ServeUI => serveUI;
        
        private void Awake()
        {
            Services.Register<UIManager>(this);
            inGameUICanvas.SetActive(false);
            gameOverUI.gameObject.SetActive(false);
            playButton.onClick.AddListener(Play);
            smashFeedback.Init();
        }

        public void Play()
        {
            Services.Get<IRoundService>().StartGame();
            mainMenuCanvas.SetActive(false);
            inGameUICanvas.SetActive(true);
        }

        public void UpdateScore(int playerScore, int opponentScore)
        {
            playerScoreText.text = playerScore.ToString();
            opponentScoreText.text = opponentScore.ToString();
        }

        public void ShowJoystick(bool show)
        {
            joystickCanvasGroup.alpha = show ? 1 : 0;
        }

        public async Task ShowSmashedFeedback(PlayerController player, bool opponentHit)
        {
            await smashFeedback.ShowAsync(player.transform, opponentHit);
        }

        public void ShowGameOverUI(bool won)
        {
            inGameUICanvas.SetActive(false);
            gameOverUI.gameObject.SetActive(true);
            _ = gameOverUI.ShowAsync(won);
        }
    }
}

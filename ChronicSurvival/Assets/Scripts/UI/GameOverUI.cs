using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Controls the Game Over Panel. Displays days survived and restart/quit options.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI daysSurvivedText;

        [Header("Interaction")]
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void OnEnable()
        {
            if (GameManager.Instance != null && daysSurvivedText != null)
            {
                int survivedDays = GameManager.Instance.CurrentDay;
                daysSurvivedText.text = $"Anda berhasil bertahan selama {survivedDays} Hari sebelum terjadi kegagalan organ.";
            }

            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.RemoveAllListeners();
                mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            }
        }

        private void OnRestartClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        private void OnMainMenuClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitToMainMenu();
            }
        }
    }
}

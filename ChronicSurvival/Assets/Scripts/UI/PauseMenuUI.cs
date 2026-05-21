using UnityEngine;
using UnityEngine.UI;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Controls the Pause Menu UI.
    /// Handles resuming and returning to the main menu.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(OnResumeButtonClicked);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(OnQuitButtonClicked);
            }
        }

        private void OnResumeButtonClicked()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePause();
            }
        }

        private void OnQuitButtonClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitToMainMenu();
            }
        }
    }
}

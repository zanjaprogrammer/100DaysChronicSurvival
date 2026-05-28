using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Battle;

namespace ChronicSurvival.UI
{
    public class ObjectiveHUDController : MonoBehaviour
    {
        [SerializeField] private RectTransform popupRoot;
        [SerializeField] private CanvasGroup popupCanvasGroup;
        [SerializeField] private TextMeshProUGUI popupTitleText;
        [SerializeField] private TextMeshProUGUI popupDescriptionText;
        [SerializeField] private RectTransform pinnedRoot;
        [SerializeField] private TextMeshProUGUI pinnedTitleText;
        [SerializeField] private TextMeshProUGUI progressLabelText;
        [SerializeField] private Image progressFillImage;
        [SerializeField] private SquadJoystickUI joystick;
        [SerializeField] private Button attackButton;

        private ImmuneSquadController squadController;
        private float popupTimer;
        private bool popupActive;

        private void Start()
        {
            squadController = FindFirstObjectByType<ImmuneSquadController>(FindObjectsInactive.Include);
            if (attackButton != null)
            {
                attackButton.onClick.RemoveListener(OnAttackPressed);
                attackButton.onClick.AddListener(OnAttackPressed);
            }

            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 0f;
            }
        }

        private void Update()
        {
            if (squadController == null)
            {
                squadController = FindFirstObjectByType<ImmuneSquadController>(FindObjectsInactive.Include);
            }

            if (squadController != null && joystick != null)
            {
                squadController.SetMoveInput(joystick.Value);
            }

            if (!popupActive || popupCanvasGroup == null || popupRoot == null || pinnedRoot == null)
            {
                return;
            }

            popupTimer += Time.deltaTime;
            float t = Mathf.Clamp01(popupTimer / 1.1f);
            popupCanvasGroup.alpha = 1f - Mathf.Clamp01((popupTimer - 0.55f) / 0.55f);
            popupRoot.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.72f, t);
            popupRoot.anchoredPosition = Vector2.Lerp(Vector2.zero, pinnedRoot.anchoredPosition, t);

            if (t >= 1f)
            {
                popupActive = false;
                popupCanvasGroup.alpha = 0f;
            }
        }

        public void ShowObjective(string title, string description)
        {
            if (popupTitleText != null) popupTitleText.text = title;
            if (popupDescriptionText != null) popupDescriptionText.text = description;
            if (pinnedTitleText != null) pinnedTitleText.text = title;

            popupTimer = 0f;
            popupActive = true;
            if (popupRoot != null)
            {
                popupRoot.anchoredPosition = Vector2.zero;
                popupRoot.localScale = Vector3.one;
            }
            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 1f;
            }
        }

        public void SetProgress(float value, string label)
        {
            if (progressFillImage != null)
            {
                progressFillImage.fillAmount = Mathf.Clamp01(value);
            }

            if (progressLabelText != null)
            {
                progressLabelText.text = label;
            }
        }

        private void OnAttackPressed()
        {
            if (squadController == null)
            {
                squadController = FindFirstObjectByType<ImmuneSquadController>(FindObjectsInactive.Include);
            }

            squadController?.RequestAttackMode();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Cards;
using System.Collections;

namespace ChronicSurvival.UI
{
    public class CardUI : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI effectsText;
        [SerializeField] private TextMeshProUGUI footerButtonText;

        [Header("Images")]
        [SerializeField] private Image bgImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image glowImage;
        [SerializeField] private Image iconPlaceholder;

        [Header("Interaction")]
        [SerializeField] private Button selectButton;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation")]
        [SerializeField] private float entranceDuration = 0.38f;
        [SerializeField] private float selectedScale = 1.08f;

        private ActionCard cardData;
        private System.Action<ActionCard> onSelect;
        private RectTransform rectTransform;
        private bool isLocked;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        public void Initialize(ActionCard card, System.Action<ActionCard> onSelectCallback, int index = 0)
        {
            cardData = card;
            onSelect = onSelectCallback;
            isLocked = false;

            if (cardData == null) return;

            Color theme = GetCardThemeColor(cardData.cardType);

            if (titleText != null) titleText.text = cardData.cardName;
            if (descriptionText != null) descriptionText.text = cardData.description;
            if (effectsText != null) effectsText.text = cardData.GetEffectsDescription();
            if (footerButtonText != null) footerButtonText.text = "Baik";

            if (bgImage != null) bgImage.color = theme;
            if (outlineImage != null) outlineImage.color = Brighten(theme, 0.35f);
            if (glowImage != null)
            {
                Color g = cardData.GetRarityColor();
                g.a = 0.35f;
                glowImage.color = g;
                glowImage.gameObject.SetActive(cardData.rarity >= CardRarity.Rare);
            }

            if (iconPlaceholder != null)
            {
                iconPlaceholder.color = Brighten(theme, 0.2f);
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnSelectClicked);
                selectButton.interactable = true;

                var colors = selectButton.colors;
                colors.normalColor = Brighten(theme, 0.15f);
                colors.highlightedColor = Brighten(theme, 0.3f);
                colors.pressedColor = Darken(theme, 0.1f);
                selectButton.colors = colors;
            }

            StopAllCoroutines();
            StartCoroutine(PlayEntrance(index));
        }

        public void SetSelectedVisual(bool selected)
        {
            if (rectTransform == null) return;
            rectTransform.localScale = Vector3.one * (selected ? selectedScale : 1f);
            if (outlineImage != null)
            {
                outlineImage.color = selected ? Color.white : Brighten(GetCardThemeColor(cardData.cardType), 0.35f);
            }
        }

        public void SetLocked(bool locked, float alpha = 0.35f)
        {
            isLocked = locked;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = locked ? alpha : 1f;
                canvasGroup.interactable = !locked;
            }
            if (selectButton != null) selectButton.interactable = !locked;
        }

        private void OnSelectClicked()
        {
            if (isLocked || cardData == null) return;
            onSelect?.Invoke(cardData);
        }

        private IEnumerator PlayEntrance(int index)
        {
            float delay = index * 0.1f;
            rectTransform.localScale = Vector3.zero;
            if (canvasGroup != null) canvasGroup.alpha = 0f;

            yield return new WaitForSecondsRealtime(delay);

            float t = 0f;
            while (t < entranceDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / entranceDuration);
                rectTransform.localScale = Vector3.one * p;
                if (canvasGroup != null) canvasGroup.alpha = p;
                yield return null;
            }
            rectTransform.localScale = Vector3.one;
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        private static Color GetCardThemeColor(CardType type)
        {
            switch (type)
            {
                case CardType.Medical: return new Color(0.12f, 0.28f, 0.52f, 0.98f);
                case CardType.Emergency: return new Color(0.45f, 0.12f, 0.18f, 0.98f);
                case CardType.Buff: return new Color(0.38f, 0.30f, 0.08f, 0.98f);
                default: return new Color(0.10f, 0.38f, 0.22f, 0.98f);
            }
        }

        private static Color Brighten(Color c, float amount) =>
            new Color(Mathf.Min(1f, c.r + amount), Mathf.Min(1f, c.g + amount), Mathf.Min(1f, c.b + amount), c.a);

        private static Color Darken(Color c, float amount) =>
            new Color(Mathf.Max(0f, c.r - amount), Mathf.Max(0f, c.g - amount), Mathf.Max(0f, c.b - amount), c.a);
    }
}

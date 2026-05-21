using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Cards;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Controls a single Card's UI representation in the selection panel.
    /// Wire-compatible with CardUI.prefab fields.
    /// </summary>
    public class CardUI : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI effectsText;

        [Header("Images")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image bgImage;
        [SerializeField] private Image outlineImage;

        [Header("Interaction")]
        [SerializeField] private Button selectButton;

        private ActionCard cardData;
        private System.Action<ActionCard> onSelect;

        public void Initialize(ActionCard card, System.Action<ActionCard> onSelectCallback)
        {
            cardData = card;
            onSelect = onSelectCallback;

            if (cardData == null)
            {
                Debug.LogError("[CardUI] ActionCard data is null!");
                return;
            }

            // Bind values
            if (titleText != null) titleText.text = cardData.cardName;
            if (descriptionText != null) descriptionText.text = cardData.description;
            if (effectsText != null)
            {
                effectsText.text = cardData.GetEffectsDescription();
                effectsText.color = cardData.rarity == CardRarity.Common ? Color.white : cardData.GetRarityColor();
            }

            if (iconImage != null)
            {
                if (cardData.icon != null)
                {
                    iconImage.sprite = cardData.icon;
                    iconImage.gameObject.SetActive(true);
                }
                else
                {
                    iconImage.gameObject.SetActive(false);
                }
            }

            // Set rarity outline color
            if (outlineImage != null)
            {
                outlineImage.color = cardData.GetRarityColor();
            }

            // Setup button click listener
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnSelectClicked);
            }
        }

        private void OnSelectClicked()
        {
            if (cardData != null)
            {
                onSelect?.Invoke(cardData);
            }
        }
    }
}

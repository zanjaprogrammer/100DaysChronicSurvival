using UnityEngine;
using System.Collections.Generic;

namespace ChronicSurvival.Cards
{
    [CreateAssetMenu(fileName = "NewCard", menuName = "ChronicSurvival/Cards/Action Card")]
    public class ActionCard : ScriptableObject
    {
        [Header("Card Info")]
        public string cardName;
        [TextArea(3, 5)]
        public string description;
        public Sprite icon;
        public CardRarity rarity = CardRarity.Common;
        public CardType cardType = CardType.Lifestyle;

        [Header("Effects")]
        public List<CardEffect> effects = new List<CardEffect>();

        [Header("Unlock Conditions")]
        public int unlockAtDay = 1;
        public bool isUnlocked = true;

        public void ApplyEffects()
        {
            if (BodyComponents.BodyComponentManager.Instance == null)
            {
                Debug.LogError("[ActionCard] BodyComponentManager not found!");
                return;
            }

            foreach (var effect in effects)
            {
                BodyComponents.BodyComponentManager.Instance.ModifyComponent(effect.componentName, effect.value);
            }

            Debug.Log($"[ActionCard] Applied card: {cardName}");
        }

        public string GetEffectsDescription()
        {
            string desc = "";
            foreach (var effect in effects)
            {
                string sign = effect.value >= 0 ? "+" : "";
                desc += $"{effect.componentName}: {sign}{effect.value}\n";
            }
            return desc;
        }

        public Color GetRarityColor()
        {
            switch (rarity)
            {
                case CardRarity.Common:
                    return new Color(0.7f, 0.7f, 0.7f); // Gray
                case CardRarity.Uncommon:
                    return new Color(0.2f, 1f, 0.2f); // Green
                case CardRarity.Rare:
                    return new Color(0.3f, 0.5f, 1f); // Blue
                case CardRarity.Epic:
                    return new Color(0.8f, 0.3f, 1f); // Purple
                case CardRarity.Legendary:
                    return new Color(1f, 0.6f, 0f); // Orange
                default:
                    return Color.white;
            }
        }
    }

    [System.Serializable]
    public class CardEffect
    {
        public string componentName;
        public float value;
    }

    public enum CardRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    public enum CardType
    {
        Lifestyle,
        Medical,
        Emergency,
        Buff,
        Debuff
    }
}

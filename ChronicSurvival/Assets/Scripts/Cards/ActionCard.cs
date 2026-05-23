using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;

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

        [Header("Immune Bonus (next battle)")]
        [Tooltip("Additive multiplier, e.g. 0.1 = +10% attack speed for immune cells")]
        [Range(0f, 0.5f)]
        public float immuneAttackSpeedBonus;
        [Range(0f, 0.5f)]
        public float immuneDamageBonus;
        [Range(0f, 0.5f)]
        public float immuneMaxHealthBonus;
        [Tooltip("Also boosts ImmuneStrength body component")]
        public float immuneStrengthBonus;

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

            if (immuneStrengthBonus != 0f)
            {
                BodyComponents.BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", immuneStrengthBonus);
            }

            EventManager.TriggerEvent(GameEvents.CARD_APPLIED, this);
            Debug.Log($"[ActionCard] Applied card: {cardName}");
        }

        public string GetEffectsDescription()
        {
            string desc = "";
            foreach (var effect in effects)
            {
                string sign = effect.value >= 0 ? "+" : "";
                desc += $"{GetComponentDisplayName(effect.componentName)}: {sign}{effect.value:0.#}\n";
            }

            if (immuneStrengthBonus != 0f)
            {
                string sign = immuneStrengthBonus >= 0 ? "+" : "";
                desc += $"Sistem Imun: {sign}{immuneStrengthBonus:0.#}\n";
            }

            if (HasImmuneCombatBonus())
            {
                desc += GetImmuneBonusDescription();
            }

            return desc.TrimEnd();
        }

        public bool HasImmuneCombatBonus()
        {
            return immuneAttackSpeedBonus > 0f || immuneDamageBonus > 0f || immuneMaxHealthBonus > 0f;
        }

        public string GetImmuneBonusDescription()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (immuneAttackSpeedBonus > 0f)
            {
                parts.Add($"ATK SPD +{Mathf.RoundToInt(immuneAttackSpeedBonus * 100f)}%");
            }
            if (immuneDamageBonus > 0f)
            {
                parts.Add($"DMG +{Mathf.RoundToInt(immuneDamageBonus * 100f)}%");
            }
            if (immuneMaxHealthBonus > 0f)
            {
                parts.Add($"HP +{Mathf.RoundToInt(immuneMaxHealthBonus * 100f)}%");
            }

            return "⚔ " + string.Join(", ", parts) + " (battle berikutnya)";
        }

        public string GetRarityLabel()
        {
            return rarity.ToString().ToUpperInvariant();
        }

        private static string GetComponentDisplayName(string key)
        {
            switch (key)
            {
                case "BloodSugar": return "Gula Darah";
                case "BloodPressure": return "Tekanan Darah";
                case "ImmuneStrength": return "Sistem Imun";
                case "SleepQuality": return "Tidur";
                case "Inflammation": return "Peradangan";
                case "InsulinEfficiency": return "Insulin";
                case "Toxicity": return "Toksin";
                case "OxygenLevel": return "Oksigen";
                case "HeartStability": return "Jantung";
                case "HormoneBalance": return "Hormon";
                default: return key;
            }
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

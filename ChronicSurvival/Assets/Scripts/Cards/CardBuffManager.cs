using UnityEngine;
using ChronicSurvival.Core;
using ChronicSurvival.UI;
using ChronicSurvival.Units;

namespace ChronicSurvival.Cards
{
    /// <summary>
    /// Stores immune combat buffs from the last selected action card.
    /// Applied to immune units at battle start and when new cells spawn.
    /// </summary>
    public class CardBuffManager : MonoBehaviour
    {
        public static CardBuffManager Instance { get; private set; }

        [Header("Active Buffs (from last card)")]
        [SerializeField] private float attackSpeedMultiplier = 1f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float maxHealthMultiplier = 1f;
        [SerializeField] private string lastCardName = "";

        public float AttackSpeedMultiplier => attackSpeedMultiplier;
        public float DamageMultiplier => damageMultiplier;
        public float MaxHealthMultiplier => maxHealthMultiplier;
        public string LastCardName => lastCardName;
        public bool HasActiveBuff =>
            attackSpeedMultiplier > 1.001f ||
            damageMultiplier > 1.001f ||
            maxHealthMultiplier > 1.001f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }

            EventManager.StartListening(GameEvents.CARD_SELECTED, OnCardSelected);
            EventManager.StartListening(GameEvents.UNIT_SPAWNED, OnUnitSpawned);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }

            EventManager.StopListening(GameEvents.CARD_SELECTED, OnCardSelected);
            EventManager.StopListening(GameEvents.UNIT_SPAWNED, OnUnitSpawned);
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Initializing)
            {
                ResetBuffs();
            }
            else if (state == GameState.Battle)
            {
                ApplyBuffsToAllImmuneUnits();
            }
        }

        private void OnCardSelected(object data)
        {
            if (data is ActionCard card)
            {
                ApplyCardBuffs(card);
            }
        }

        private void OnUnitSpawned(object data)
        {
            if (data is Unit unit)
            {
                ApplyBuffsToUnit(unit);
            }
        }

        public void ApplyCardBuffs(ActionCard card)
        {
            if (card == null)
            {
                ResetBuffs();
                return;
            }

            attackSpeedMultiplier = 1f + Mathf.Max(0f, card.immuneAttackSpeedBonus);
            damageMultiplier = 1f + Mathf.Max(0f, card.immuneDamageBonus);
            maxHealthMultiplier = 1f + Mathf.Max(0f, card.immuneMaxHealthBonus);
            lastCardName = card.cardName;

            Debug.Log($"[CardBuffManager] Buffs for next battle — ATK SPD x{attackSpeedMultiplier:F2}, DMG x{damageMultiplier:F2}, HP x{maxHealthMultiplier:F2} ({lastCardName})");
            UpdateActiveEventUI();
        }

        public void ResetBuffs()
        {
            attackSpeedMultiplier = 1f;
            damageMultiplier = 1f;
            maxHealthMultiplier = 1f;
            lastCardName = "";
            UpdateActiveEventUI();
        }

        private void UpdateActiveEventUI()
        {
            var ui = FindFirstObjectByType<ActiveEventUI>(FindObjectsInactive.Include);
            if (ui == null) return;

            if (HasActiveBuff)
            {
                ui.ShowEvent($"Kartu: {lastCardName}", GetBuffSummary());
            }
            else
            {
                ui.HideEvent();
            }
        }

        public void ApplyBuffsToUnit(Unit unit)
        {
            if (unit == null || unit.Team != UnitTeam.Immune || !HasActiveBuff)
            {
                return;
            }

            unit.ApplyCombatMultipliers(attackSpeedMultiplier, damageMultiplier, maxHealthMultiplier);
        }

        public void ApplyBuffsToAllImmuneUnits()
        {
            if (!HasActiveBuff)
            {
                return;
            }

            var units = FindObjectsByType<ImmuneCell>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var cell in units)
            {
                ApplyBuffsToUnit(cell);
            }
        }

        public string GetBuffSummary()
        {
            if (!HasActiveBuff)
            {
                return "";
            }

            var parts = new System.Collections.Generic.List<string>();
            if (attackSpeedMultiplier > 1.001f)
            {
                parts.Add($"ATK SPD +{Mathf.RoundToInt((attackSpeedMultiplier - 1f) * 100f)}%");
            }
            if (damageMultiplier > 1.001f)
            {
                parts.Add($"DMG +{Mathf.RoundToInt((damageMultiplier - 1f) * 100f)}%");
            }
            if (maxHealthMultiplier > 1.001f)
            {
                parts.Add($"HP +{Mathf.RoundToInt((maxHealthMultiplier - 1f) * 100f)}%");
            }

            return string.Join(" · ", parts);
        }
    }
}

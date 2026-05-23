using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using ChronicSurvival.Core;

namespace ChronicSurvival.Cards
{
    public class CardManager : MonoBehaviour
    {
        public static CardManager Instance { get; private set; }

        [Header("Card Pool")]
        [SerializeField] private List<ActionCard> allCards = new List<ActionCard>();
        [SerializeField] private ActionCardDatabase cardDatabase;
        [SerializeField] private bool autoLoadFromDatabase = true;
        [SerializeField] private bool autoLoadFromResources = true;

        [Header("Settings")]
        [SerializeField] private int cardsPerDraw = 3;
        [SerializeField] private bool allowDuplicates = false;

        [Header("Rarity Weights")]
        [SerializeField] private float commonWeight = 60f;
        [SerializeField] private float uncommonWeight = 25f;
        [SerializeField] private float rareWeight = 10f;
        [SerializeField] private float epicWeight = 4f;
        [SerializeField] private float legendaryWeight = 1f;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        private List<ActionCard> currentCardOptions = new List<ActionCard>();
        private List<ActionCard> availableCards = new List<ActionCard>();

        public System.Action<List<ActionCard>> OnCardsDrawn;
        public System.Action<ActionCard> OnCardSelected;

        public List<ActionCard> CurrentCardOptions => currentCardOptions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeCardPool();
        }

        private void InitializeCardPool()
        {
            PopulatePoolIfEmpty();
            UpdateAvailableCards();

            if (debugMode)
            {
                Debug.Log($"[CardManager] Initialized with {allCards.Count} total cards, {availableCards.Count} available");
            }
        }

        private void PopulatePoolIfEmpty()
        {
            if (allCards != null && allCards.Count > 0)
            {
                return;
            }

            allCards = new List<ActionCard>();

            if (autoLoadFromDatabase && cardDatabase != null && cardDatabase.cards.Count > 0)
            {
                allCards.AddRange(cardDatabase.cards);
            }

            if (autoLoadFromResources && allCards.Count == 0)
            {
                var fromResources = Resources.LoadAll<ActionCard>("Cards");
                if (fromResources != null && fromResources.Length > 0)
                {
                    allCards.AddRange(fromResources);
                }
            }

            if (allCards.Count == 0)
            {
                AddRuntimeDefaultCards();
            }
        }

        private void AddRuntimeDefaultCards()
        {
            allCards.Add(CreateRuntimeCard("Istirahat Taktis", "Pulihkan energi dan kualitas tidur tubuh.", CardType.Lifestyle, CardRarity.Common,
                new CardEffect { componentName = "Energy", value = 12f },
                new CardEffect { componentName = "SleepQuality", value = 10f }));
            allCards.Add(CreateRuntimeCard("Hidrasi Klinis", "Stabilkan cairan tubuh dan bantu mengurangi toksin.", CardType.Medical, CardRarity.Common,
                new CardEffect { componentName = "Hydration", value = 18f },
                new CardEffect { componentName = "Toxicity", value = -8f }));
            allCards.Add(CreateRuntimeCard("Nutrisi Rendah Gula", "Tekan lonjakan gula darah dan bantu insulin bekerja.", CardType.Lifestyle, CardRarity.Uncommon,
                new CardEffect { componentName = "BloodSugar", value = -18f },
                new CardEffect { componentName = "InsulinEfficiency", value = 10f }));
            allCards.Add(CreateRuntimeCard("Latihan Pernapasan", "Turunkan stress dan tekanan darah.", CardType.Lifestyle, CardRarity.Common,
                new CardEffect { componentName = "Stress", value = -15f },
                new CardEffect { componentName = "BloodPressure", value = -8f }));
            allCards.Add(CreateRuntimeCard("Terapi Anti Inflamasi", "Redakan peradangan dan bantu sistem imun.", CardType.Medical, CardRarity.Rare,
                new CardEffect { componentName = "Inflammation", value = -16f },
                new CardEffect { componentName = "ImmuneStrength", value = 8f }));
        }

        private ActionCard CreateRuntimeCard(string cardName, string description, CardType type, CardRarity rarity, params CardEffect[] effects)
        {
            ActionCard card = ScriptableObject.CreateInstance<ActionCard>();
            card.cardName = cardName;
            card.description = description;
            card.cardType = type;
            card.rarity = rarity;
            card.unlockAtDay = 1;
            card.isUnlocked = true;
            card.effects = new List<CardEffect>(effects);
            return card;
        }

        public void SetCardDatabase(ActionCardDatabase database)
        {
            cardDatabase = database;
            if (database != null)
            {
                allCards = new List<ActionCard>(database.cards);
                UpdateAvailableCards();
            }
        }

        private void UpdateAvailableCards()
        {
            int currentDay = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;

            availableCards = allCards
                .Where(card => card != null && card.isUnlocked && card.unlockAtDay <= currentDay && card.cardType != CardType.Debuff)
                .ToList();
        }

        public List<ActionCard> DrawCards()
        {
            UpdateAvailableCards();

            if (availableCards.Count == 0)
            {
                Debug.LogWarning("[CardManager] No available cards to draw!");
                return new List<ActionCard>();
            }

            currentCardOptions.Clear();

            int cardsToDraw = Mathf.Min(cardsPerDraw, availableCards.Count);
            List<ActionCard> tempPool = new List<ActionCard>(availableCards);

            for (int i = 0; i < cardsToDraw; i++)
            {
                ActionCard drawnCard = DrawCardByRarity(tempPool);
                
                if (drawnCard != null)
                {
                    currentCardOptions.Add(drawnCard);

                    if (!allowDuplicates)
                    {
                        tempPool.Remove(drawnCard);
                    }
                }
            }

            OnCardsDrawn?.Invoke(currentCardOptions);
            EventManager.TriggerEvent(GameEvents.CARD_POOL_GENERATED, currentCardOptions);

            if (debugMode)
            {
                Debug.Log($"[CardManager] Drew {currentCardOptions.Count} cards");
            }

            return currentCardOptions;
        }

        private ActionCard DrawCardByRarity(List<ActionCard> pool)
        {
            if (pool.Count == 0) return null;

            CardRarity targetRarity = RollRarity();
            
            // Try to find card with target rarity
            List<ActionCard> cardsOfRarity = pool.Where(c => c.rarity == targetRarity).ToList();
            
            // If no cards of that rarity, get any card
            if (cardsOfRarity.Count == 0)
            {
                cardsOfRarity = pool;
            }

            return cardsOfRarity[Random.Range(0, cardsOfRarity.Count)];
        }

        private CardRarity RollRarity()
        {
            float totalWeight = commonWeight + uncommonWeight + rareWeight + epicWeight + legendaryWeight;
            float roll = Random.Range(0f, totalWeight);

            if (roll < commonWeight)
                return CardRarity.Common;
            else if (roll < commonWeight + uncommonWeight)
                return CardRarity.Uncommon;
            else if (roll < commonWeight + uncommonWeight + rareWeight)
                return CardRarity.Rare;
            else if (roll < commonWeight + uncommonWeight + rareWeight + epicWeight)
                return CardRarity.Epic;
            else
                return CardRarity.Legendary;
        }

        public void SelectCard(ActionCard card)
        {
            if (card == null)
            {
                Debug.LogError("[CardManager] Tried to select null card!");
                return;
            }

            if (!currentCardOptions.Contains(card))
            {
                Debug.LogWarning("[CardManager] Selected card is not in current options!");
                return;
            }

            card.ApplyEffects();

            OnCardSelected?.Invoke(card);
            EventManager.TriggerEvent(GameEvents.CARD_SELECTED, card);

            if (debugMode)
            {
                Debug.Log($"[CardManager] Selected card: {card.cardName}");
            }

            currentCardOptions.Clear();
        }

        public void AddCardToPool(ActionCard card)
        {
            if (card != null && !allCards.Contains(card))
            {
                allCards.Add(card);
                UpdateAvailableCards();

                if (debugMode)
                {
                    Debug.Log($"[CardManager] Added card to pool: {card.cardName}");
                }
            }
        }

        public void UnlockCard(ActionCard card)
        {
            if (card != null)
            {
                card.isUnlocked = true;
                UpdateAvailableCards();

                if (debugMode)
                {
                    Debug.Log($"[CardManager] Unlocked card: {card.cardName}");
                }
            }
        }

        public int GetAvailableCardCount()
        {
            return availableCards.Count;
        }

        public List<ActionCard> GetAllCards()
        {
            return new List<ActionCard>(allCards);
        }
    }
}

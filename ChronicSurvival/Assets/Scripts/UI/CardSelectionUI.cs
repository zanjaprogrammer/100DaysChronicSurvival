using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Cards;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Manages the Card Selection Panel. Draws 3 cards and displays them.
    /// Handles card selection and moves the game state forward.
    /// </summary>
    public class CardSelectionUI : MonoBehaviour
    {
        [Header("Prefabs & Layout")]
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private Transform cardsContainer;

        private List<GameObject> activeCardObjects = new List<GameObject>();

        private void OnEnable()
        {
            // Draw cards and show them when the panel is enabled
            DisplayCardChoices();
        }

        public void DisplayCardChoices()
        {
            // Clear any previously instantiated card elements
            ClearChoices();

            if (CardManager.Instance == null)
            {
                Debug.LogError("[CardSelectionUI] CardManager Instance is missing!");
                return;
            }

            if (cardUIPrefab == null || cardsContainer == null)
            {
                Debug.LogWarning("[CardSelectionUI] Prefab or Container not assigned!");
                return;
            }

            // Draw cards from the pool
            List<ActionCard> drawnCards = CardManager.Instance.DrawCards();

            if (drawnCards == null || drawnCards.Count == 0)
            {
                Debug.LogWarning("[CardSelectionUI] No cards were drawn!");
                // Failsafe: Continue directly to RandomEvent if card draw fails
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EndCardSelection();
                }
                return;
            }

            // Instantiate and initialize CardUI components
            foreach (var card in drawnCards)
            {
                GameObject cardObj = Instantiate(cardUIPrefab, cardsContainer);
                CardUI cardUI = cardObj.GetComponent<CardUI>();

                if (cardUI != null)
                {
                    cardUI.Initialize(card, OnCardSelected);
                    activeCardObjects.Add(cardObj);
                }
                else
                {
                    Debug.LogError("[CardSelectionUI] Prefab does not have a CardUI component!");
                    Destroy(cardObj);
                }
            }
        }

        private void OnCardSelected(ActionCard card)
        {
            if (CardManager.Instance != null)
            {
                CardManager.Instance.SelectCard(card);
            }

            // Transition state to Random Event
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndCardSelection();
            }
        }

        private void ClearChoices()
        {
            foreach (var obj in activeCardObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            activeCardObjects.Clear();

            // Double check and clear all children dynamically to be safe
            if (cardsContainer != null)
            {
                foreach (Transform child in cardsContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void OnDisable()
        {
            ClearChoices();
        }
    }
}

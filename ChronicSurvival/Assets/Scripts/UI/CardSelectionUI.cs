using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using ChronicSurvival.Cards;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Center-screen card selection modal with full-screen dim overlay.
    /// </summary>
    public class CardSelectionUI : MonoBehaviour
    {
        [Header("Prefabs & Layout")]
        [SerializeField] private GameObject cardUIPrefab;
        [SerializeField] private Transform cardsContainer;

        [Header("Modal")]
        [SerializeField] private Image fullScreenDim;
        [SerializeField] private RectTransform modalPanel;
        [SerializeField] private CanvasGroup modalCanvasGroup;
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI subtitleText;

        [Header("Timing")]
        [SerializeField] private float overlayFadeDuration = 0.35f;
        [SerializeField] private float modalPopDuration = 0.4f;
        [SerializeField] private float confirmDelay = 0.45f;
        [SerializeField] private float dimTargetAlpha = 0.82f;

        private readonly List<GameObject> activeCardObjects = new List<GameObject>();
        private readonly List<CardUI> activeCardUIs = new List<CardUI>();
        private bool isSelecting;

        /// <summary>Called by GameplayUICreator after building the modal in code.</summary>
        public void ConfigureRuntime(
            GameObject cardPrefabRef,
            Transform container,
            Image dim,
            RectTransform modal,
            CanvasGroup modalGroup,
            TextMeshProUGUI header,
            TextMeshProUGUI subtitle)
        {
            cardUIPrefab = cardPrefabRef;
            cardsContainer = container;
            fullScreenDim = dim;
            modalPanel = modal;
            modalCanvasGroup = modalGroup;
            headerText = header;
            subtitleText = subtitle;
        }

        private void OnEnable()
        {
            isSelecting = false;
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.CardSelection)
            {
                DisplayCardChoices();
            }
        }

        public void DisplayCardChoices()
        {
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

            UpdateHeaderTexts();

            List<ActionCard> drawnCards = CardManager.Instance.DrawCards();
            if (drawnCards == null || drawnCards.Count == 0)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EndCardSelection();
                }
                return;
            }

            StopAllCoroutines();
            StartCoroutine(ShowModalSequence(drawnCards));
        }

        private void UpdateHeaderTexts()
        {
            int day = GameManager.Instance != null ? GameManager.Instance.CurrentDay : 1;
            if (headerText != null)
            {
                headerText.text = "PILIH 1 KARTU TINDAKAN";
            }
            if (subtitleText != null)
            {
                subtitleText.text = $"Hari {day} — Pilih keputusan gaya hidup untuk tubuhmu";
            }
        }

        private IEnumerator ShowModalSequence(List<ActionCard> drawnCards)
        {
            if (modalPanel != null)
            {
                modalPanel.localScale = Vector3.one * 0.85f;
            }
            if (modalCanvasGroup != null)
            {
                modalCanvasGroup.alpha = 0f;
            }
            if (fullScreenDim != null)
            {
                Color c = fullScreenDim.color;
                c.a = 0f;
                fullScreenDim.color = c;
            }

            float t = 0f;
            while (t < overlayFadeDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = t / overlayFadeDuration;
                if (fullScreenDim != null)
                {
                    Color c = fullScreenDim.color;
                    c.a = dimTargetAlpha * p;
                    fullScreenDim.color = c;
                }
                yield return null;
            }

            if (fullScreenDim != null)
            {
                Color c = fullScreenDim.color;
                c.a = dimTargetAlpha;
                fullScreenDim.color = c;
            }

            for (int i = 0; i < drawnCards.Count; i++)
            {
                GameObject cardObj = Instantiate(cardUIPrefab, cardsContainer);
                CardUI cardUI = cardObj.GetComponent<CardUI>();
                if (cardUI != null)
                {
                    ActionCard captured = drawnCards[i];
                    cardUI.Initialize(captured, c => OnCardSelected(c, cardUI), i);
                    activeCardObjects.Add(cardObj);
                    activeCardUIs.Add(cardUI);
                }
                else
                {
                    Destroy(cardObj);
                }
            }

            t = 0f;
            while (t < modalPopDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / modalPopDuration);
                if (modalCanvasGroup != null) modalCanvasGroup.alpha = p;
                if (modalPanel != null) modalPanel.localScale = Vector3.one * Mathf.Lerp(0.85f, 1f, p);
                yield return null;
            }

            if (modalCanvasGroup != null) modalCanvasGroup.alpha = 1f;
            if (modalPanel != null) modalPanel.localScale = Vector3.one;
        }

        private void OnCardSelected(ActionCard card, CardUI chosenUI)
        {
            if (isSelecting) return;
            isSelecting = true;

            foreach (var ui in activeCardUIs)
            {
                if (ui == null) continue;
                bool chosen = ui == chosenUI;
                ui.SetSelectedVisual(chosen);
                ui.SetLocked(!chosen, chosen ? 1f : 0.2f);
            }

            if (CardManager.Instance != null)
            {
                CardManager.Instance.SelectCard(card);
            }

            StartCoroutine(ConfirmAndClose());
        }

        private IEnumerator ConfirmAndClose()
        {
            yield return new WaitForSecondsRealtime(confirmDelay);

            float t = 0f;
            float dur = 0.25f;
            float startModalAlpha = modalCanvasGroup != null ? modalCanvasGroup.alpha : 1f;
            float startDim = fullScreenDim != null ? fullScreenDim.color.a : 0f;

            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                float p = t / dur;
                if (modalCanvasGroup != null) modalCanvasGroup.alpha = Mathf.Lerp(startModalAlpha, 0f, p);
                if (fullScreenDim != null)
                {
                    Color c = fullScreenDim.color;
                    c.a = Mathf.Lerp(startDim, 0f, p);
                    fullScreenDim.color = c;
                }
                if (modalPanel != null)
                {
                    modalPanel.localScale = Vector3.one * Mathf.Lerp(1f, 0.9f, p);
                }
                yield return null;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndCardSelection();
            }

            isSelecting = false;
        }

        private void ClearChoices()
        {
            foreach (var obj in activeCardObjects)
            {
                if (obj != null) Destroy(obj);
            }
            activeCardObjects.Clear();
            activeCardUIs.Clear();

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
            StopAllCoroutines();
            ClearChoices();
            isSelecting = false;

            if (fullScreenDim != null)
            {
                Color c = fullScreenDim.color;
                c.a = 0f;
                fullScreenDim.color = c;
            }
        }
    }
}

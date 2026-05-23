using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Bottom bar with compact body stats + expandable full panel (panah atas).
    /// </summary>
    public class BodyStatsDrawerUI : MonoBehaviour
    {
        [Header("Compact Bar")]
        [SerializeField] private RectTransform compactBarRoot;
        [SerializeField] private Transform compactChipsContainer;
        [SerializeField] private GameObject compactChipPrefab;

        [Header("Expanded Drawer")]
        [SerializeField] private RectTransform expandedDrawer;
        [SerializeField] private BodyStatsPanel expandedStatsPanel;
        [SerializeField] private CanvasGroup expandedCanvasGroup;

        [Header("Toggle")]
        [SerializeField] private Button expandButton;
        [SerializeField] private TextMeshProUGUI expandArrowText;

        [Header("Layout")]
        [SerializeField] private float expandedHeight = 340f;
        [SerializeField] private float animDuration = 0.28f;
        [SerializeField] private List<string> compactStatKeys = new List<string>
        {
            "Energy", "BloodSugar", "ImmuneStrength", "Inflammation", "Stress"
        };

        private readonly List<CompactStatChipUI> chips = new List<CompactStatChipUI>();
        private bool isExpanded;
        private float collapsedDrawerY;
        private float expandedDrawerY;
        private Coroutine animRoutine;

        private void Start()
        {
            if (expandedDrawer != null)
            {
                collapsedDrawerY = -expandedHeight;
                expandedDrawerY = 62f;
                expandedDrawer.anchoredPosition = new Vector2(expandedDrawer.anchoredPosition.x, collapsedDrawerY);
                if (expandedCanvasGroup != null)
                {
                    expandedCanvasGroup.alpha = 0f;
                    expandedCanvasGroup.interactable = false;
                    expandedCanvasGroup.blocksRaycasts = false;
                }
            }

            BuildCompactChips();

            if (expandButton != null)
            {
                expandButton.onClick.AddListener(ToggleExpanded);
            }

            UpdateArrowVisual();
        }

        private void BuildCompactChips()
        {
            if (BodyComponentManager.Instance == null || compactChipsContainer == null)
            {
                return;
            }

            foreach (Transform child in compactChipsContainer)
            {
                Destroy(child.gameObject);
            }
            chips.Clear();

            foreach (string key in compactStatKeys)
            {
                BodyComponent comp = BodyComponentManager.Instance.GetComponent(key);
                if (comp == null) continue;

                GameObject chipObj;
                if (compactChipPrefab != null)
                {
                    chipObj = Instantiate(compactChipPrefab, compactChipsContainer);
                }
                else
                {
                    chipObj = new GameObject($"Chip_{key}", typeof(RectTransform));
                    chipObj.transform.SetParent(compactChipsContainer, false);
                }

                CompactStatChipUI chip = chipObj.GetComponent<CompactStatChipUI>();
                if (chip != null)
                {
                    chip.Bind(key, comp);
                    chips.Add(chip);
                }
            }
        }

        public void ToggleExpanded()
        {
            SetExpanded(!isExpanded);
        }

        public void SetExpanded(bool expanded)
        {
            if (isExpanded == expanded) return;
            isExpanded = expanded;

            if (animRoutine != null)
            {
                StopCoroutine(animRoutine);
            }
            animRoutine = StartCoroutine(AnimateDrawer());
            UpdateArrowVisual();
        }

        private void UpdateArrowVisual()
        {
            if (expandArrowText != null)
            {
                expandArrowText.text = isExpanded ? "v" : "^";
            }
        }

        private IEnumerator AnimateDrawer()
        {
            if (expandedDrawer == null) yield break;

            float targetY = isExpanded ? expandedDrawerY : collapsedDrawerY;
            float startY = expandedDrawer.anchoredPosition.y;
            float targetAlpha = isExpanded ? 1f : 0f;
            float startAlpha = expandedCanvasGroup != null ? expandedCanvasGroup.alpha : 0f;

            if (isExpanded && expandedCanvasGroup != null)
            {
                expandedCanvasGroup.interactable = true;
                expandedCanvasGroup.blocksRaycasts = true;
            }

            float t = 0f;
            while (t < animDuration)
            {
                t += Time.unscaledDeltaTime;
                float p = Mathf.SmoothStep(0f, 1f, t / animDuration);
                expandedDrawer.anchoredPosition = new Vector2(
                    expandedDrawer.anchoredPosition.x,
                    Mathf.Lerp(startY, targetY, p));

                if (expandedCanvasGroup != null)
                {
                    expandedCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, p);
                }
                yield return null;
            }

            expandedDrawer.anchoredPosition = new Vector2(expandedDrawer.anchoredPosition.x, targetY);
            if (expandedCanvasGroup != null)
            {
                expandedCanvasGroup.alpha = targetAlpha;
                if (!isExpanded)
                {
                    expandedCanvasGroup.interactable = false;
                    expandedCanvasGroup.blocksRaycasts = false;
                }
            }
        }
    }
}

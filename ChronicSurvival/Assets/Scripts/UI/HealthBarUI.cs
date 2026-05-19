using UnityEngine;
using UnityEngine.UI;
using ChronicSurvival.Units;

namespace ChronicSurvival.UI
{
    public class HealthBarUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Unit targetUnit;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image backgroundImage;

        [Header("Settings")]
        [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private bool hideWhenDead = true;
        [SerializeField] private float smoothSpeed = 5f;

        [Header("Colors")]
        [SerializeField] private Color fullHealthColor = Color.green;
        [SerializeField] private Color midHealthColor = Color.yellow;
        [SerializeField] private Color lowHealthColor = Color.red;
        [SerializeField] private Gradient healthGradient;

        private Camera mainCamera;
        private Canvas canvas;
        private float targetFillAmount;
        private bool useGradient = false;

        private void Awake()
        {
            mainCamera = Camera.main;
            canvas = GetComponentInParent<Canvas>();

            if (healthGradient != null && healthGradient.colorKeys.Length > 0)
            {
                useGradient = true;
            }
        }

        private void Start()
        {
            if (targetUnit != null)
            {
                SetupUnit(targetUnit);
            }

            UpdateHealthBar();
        }

        private void LateUpdate()
        {
            if (targetUnit == null || targetUnit.IsDead)
            {
                if (hideWhenDead)
                {
                    gameObject.SetActive(false);
                }
                return;
            }

            UpdatePosition();
            UpdateHealthBar();
        }

        public void SetupUnit(Unit unit)
        {
            targetUnit = unit;

            if (targetUnit != null)
            {
                targetUnit.OnDamaged += OnUnitDamaged;
                targetUnit.OnDeath += OnUnitDeath;
            }

            UpdateHealthBar();
        }

        private void UpdatePosition()
        {
            if (targetUnit == null || mainCamera == null) return;

            Vector3 worldPos = targetUnit.transform.position + offset;

            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                transform.position = worldPos;
            }
            else
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                transform.position = screenPos;
            }
        }

        private void UpdateHealthBar()
        {
            if (targetUnit == null || fillImage == null) return;

            float healthPercent = targetUnit.CurrentHealth / targetUnit.MaxHealth;
            targetFillAmount = healthPercent;

            fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);

            UpdateColor(healthPercent);

            if (hideWhenFull && healthPercent >= 0.99f)
            {
                canvas.enabled = false;
            }
            else
            {
                canvas.enabled = true;
            }
        }

        private void UpdateColor(float healthPercent)
        {
            if (fillImage == null) return;

            if (useGradient)
            {
                fillImage.color = healthGradient.Evaluate(healthPercent);
            }
            else
            {
                if (healthPercent > 0.6f)
                    fillImage.color = fullHealthColor;
                else if (healthPercent > 0.3f)
                    fillImage.color = midHealthColor;
                else
                    fillImage.color = lowHealthColor;
            }
        }

        private void OnUnitDamaged(float damage)
        {
            UpdateHealthBar();
        }

        private void OnUnitDeath(Unit unit)
        {
            if (hideWhenDead)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (targetUnit != null)
            {
                targetUnit.OnDamaged -= OnUnitDamaged;
                targetUnit.OnDeath -= OnUnitDeath;
            }
        }
    }
}

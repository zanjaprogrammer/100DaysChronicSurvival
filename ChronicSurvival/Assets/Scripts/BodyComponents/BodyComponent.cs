using UnityEngine;
using System;

namespace ChronicSurvival.BodyComponents
{
    [CreateAssetMenu(fileName = "New Body Component", menuName = "Chronic Survival/Body Component")]
    public class BodyComponent : ScriptableObject
    {
        [Header("Basic Info")]
        public string componentName;
        [TextArea(3, 5)]
        public string description;
        public Sprite icon;

        [Header("Value Settings")]
        public float minValue = 0f;
        public float maxValue = 100f;
        public float optimalMin = 60f;
        public float optimalMax = 80f;
        public float criticalLow = 20f;
        public float criticalHigh = 80f;

        [Header("Starting Value")]
        public float startingValue = 70f;

        [Header("Passive Changes")]
        public float passiveDecay = -1f;
        public float decayInterval = 1f;

        [Header("Visual")]
        public Color normalColor = Color.green;
        public Color warningColor = Color.yellow;
        public Color dangerColor = Color.red;

        public float CurrentValue { get; private set; }
        public ComponentState CurrentState { get; private set; }

        public event Action<float> OnValueChanged;
        public event Action<ComponentState> OnStateChanged;
        public event Action OnCritical;

        public void Initialize()
        {
            CurrentValue = startingValue;
            UpdateState();
        }

        public void SetValue(float value)
        {
            float oldValue = CurrentValue;
            CurrentValue = Mathf.Clamp(value, minValue, maxValue);

            if (Mathf.Abs(oldValue - CurrentValue) > 0.01f)
            {
                OnValueChanged?.Invoke(CurrentValue);
                UpdateState();
            }
        }

        public void ModifyValue(float delta)
        {
            SetValue(CurrentValue + delta);
        }

        public void ApplyPassiveDecay(float deltaTime)
        {
            if (passiveDecay != 0)
            {
                ModifyValue(passiveDecay * (deltaTime / decayInterval));
            }
        }

        private void UpdateState()
        {
            ComponentState newState = CalculateState();

            if (newState != CurrentState)
            {
                CurrentState = newState;
                OnStateChanged?.Invoke(CurrentState);

                if (CurrentState == ComponentState.Critical)
                {
                    OnCritical?.Invoke();
                }
            }
        }

        private ComponentState CalculateState()
        {
            if (CurrentValue <= criticalLow || CurrentValue >= criticalHigh)
                return ComponentState.Critical;

            if (CurrentValue >= optimalMin && CurrentValue <= optimalMax)
                return ComponentState.Optimal;

            if (CurrentValue < optimalMin || CurrentValue > optimalMax)
                return ComponentState.Warning;

            return ComponentState.Normal;
        }

        public Color GetCurrentColor()
        {
            switch (CurrentState)
            {
                case ComponentState.Optimal:
                case ComponentState.Normal:
                    return normalColor;
                case ComponentState.Warning:
                    return warningColor;
                case ComponentState.Critical:
                    return dangerColor;
                default:
                    return normalColor;
            }
        }

        public float GetNormalizedValue()
        {
            return (CurrentValue - minValue) / (maxValue - minValue);
        }

        public bool IsOptimal()
        {
            return CurrentState == ComponentState.Optimal;
        }

        public bool IsCritical()
        {
            return CurrentState == ComponentState.Critical;
        }
    }

    public enum ComponentState
    {
        Optimal,
        Normal,
        Warning,
        Critical
    }
}

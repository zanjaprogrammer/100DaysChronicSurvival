using System.Reflection;
using UnityEngine;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Assigns private [SerializeField] references at runtime (NeonShooter-style code-built UI).
    /// </summary>
    internal static class UiFieldBinder
    {
        public static void Set(MonoBehaviour target, string fieldName, object value)
        {
            if (target == null) return;

            FieldInfo field = target.GetType().GetField(
                fieldName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (field == null)
            {
                Debug.LogWarning($"[UiFieldBinder] Field '{fieldName}' tidak ditemukan di {target.GetType().Name}");
                return;
            }

            field.SetValue(target, value);
        }
    }
}

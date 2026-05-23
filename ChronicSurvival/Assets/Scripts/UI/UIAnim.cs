using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ChronicSurvival.UI
{
    public static class UIAnim
    {
        public static IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration, bool useUnscaled = true)
        {
            if (group == null) yield break;

            float start = group.alpha;
            float t = 0f;
            while (t < duration)
            {
                t += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
                group.alpha = Mathf.Lerp(start, targetAlpha, EaseOutCubic(p));
                yield return null;
            }
            group.alpha = targetAlpha;
        }

        public static IEnumerator FadeImage(UnityEngine.UI.Image image, float targetAlpha, float duration, bool useUnscaled = true)
        {
            if (image == null) yield break;

            Color c = image.color;
            float start = c.a;
            float t = 0f;
            while (t < duration)
            {
                t += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
                c.a = Mathf.Lerp(start, targetAlpha, EaseOutCubic(p));
                image.color = c;
                yield return null;
            }
            c.a = targetAlpha;
            image.color = c;
        }

        public static IEnumerator ScaleRect(RectTransform rect, Vector3 targetScale, float duration, bool useUnscaled = true)
        {
            if (rect == null) yield break;

            Vector3 start = rect.localScale;
            float t = 0f;
            while (t < duration)
            {
                t += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
                float eased = EaseOutCubic(p);
                rect.localScale = Vector3.Lerp(start, targetScale, eased);
                yield return null;
            }
            rect.localScale = targetScale;
        }

        public static IEnumerator SlideAnchoredY(RectTransform rect, float targetY, float duration, bool useUnscaled = true)
        {
            if (rect == null) yield break;

            Vector2 start = rect.anchoredPosition;
            Vector2 end = new Vector2(start.x, targetY);
            float t = 0f;
            while (t < duration)
            {
                t += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
                rect.anchoredPosition = Vector2.Lerp(start, end, EaseOutCubic(p));
                yield return null;
            }
            rect.anchoredPosition = end;
        }

        public static float EaseOutCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        public static float EaseOutQuad(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - (1f - t) * (1f - t);
        }

        public static float EaseOutBack(float t)
        {
            t = Mathf.Clamp01(t);
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }

        public static IEnumerator SlideAnchored(RectTransform rect, Vector2 target, float duration, bool useUnscaled = true)
        {
            if (rect == null) yield break;

            Vector2 start = rect.anchoredPosition;
            float t = 0f;
            while (t < duration)
            {
                t += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                float p = duration > 0f ? EaseOutCubic(t / duration) : 1f;
                rect.anchoredPosition = Vector2.LerpUnclamped(start, target, p);
                yield return null;
            }
            rect.anchoredPosition = target;
        }

        public static IEnumerator PulseGraphic(Graphic graphic, Color a, Color b, float duration, int loops, bool useUnscaled = true)
        {
            if (graphic == null) yield break;
            int total = Mathf.Max(1, loops);
            for (int i = 0; i < total; i++)
            {
                float t = 0f;
                while (t < duration)
                {
                    t += useUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;
                    float p = duration > 0f ? Mathf.PingPong(t / duration, 1f) : 1f;
                    graphic.color = Color.Lerp(a, b, p);
                    yield return null;
                }
            }
            graphic.color = a;
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class GuitarNeckUI : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private RectTransform neckArea;
    [SerializeField] private RectTransform[] stringVisuals; // Referencias a los objetos UI de las cuerdas

    private Color normalStringColor = Color.white;
    private Color playedStringColor = Color.yellow;
    private float stringAnimationDuration = 0.2f;

    public void AnimateString(int stringIndex)
    {
        if (stringIndex < 0 || stringIndex >= stringVisuals.Length) return;

        // Cambiar color de la cuerda
        Image stringImage = stringVisuals[stringIndex].GetComponent<Image>();
        if (stringImage != null)
        {
            StartCoroutine(AnimateStringColor(stringImage));
        }

        // Animar movimiento de la cuerda
        StartCoroutine(AnimateStringMovement(stringVisuals[stringIndex]));
    }

    private System.Collections.IEnumerator AnimateStringColor(Image stringImage)
    {
        stringImage.color = playedStringColor;

        float elapsed = 0;
        while (elapsed < stringAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stringAnimationDuration;
            stringImage.color = Color.Lerp(playedStringColor, normalStringColor, t);
            yield return null;
        }

        stringImage.color = normalStringColor;
    }

    private System.Collections.IEnumerator AnimateStringMovement(RectTransform stringTransform)
    {
        Vector2 originalPosition = stringTransform.anchoredPosition;
        float amplitude = 5f; // Qué tanto se desplaza la cuerda

        float elapsed = 0;
        while (elapsed < stringAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stringAnimationDuration;

            // Movimiento oscilatorio que se atenúa con el tiempo
            float offset = amplitude * Mathf.Sin(t * Mathf.PI * 8) * (1 - t);
            stringTransform.anchoredPosition = new Vector2(originalPosition.x + offset, originalPosition.y);

            yield return null;
        }

        stringTransform.anchoredPosition = originalPosition;
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuitarNeckUI : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private RectTransform neckArea;
    [SerializeField] private RectTransform[] stringVisuals; // Referencias a los objetos UI de las cuerdas

    private Color normalStringColor = Color.white;
    private Color playedStringColor = Color.yellow;
    private float stringAnimationDuration = 0.2f;

    private void Start()
    {
        if (neckArea == null)
        {
            neckArea = GetComponent<RectTransform>();
        }

        if (stringVisuals == null || stringVisuals.Length == 0)
        {
            CreateStringVisuals();
        }
    }

    private void CreateStringVisuals()
    {
        // Crear cuerdas visuales si no existen
        stringVisuals = new RectTransform[6];

        for (int i = 0; i < 6; i++)
        {
            GameObject stringObj = new GameObject($"String_{i + 1}");
            stringObj.transform.SetParent(transform);

            RectTransform rectTransform = stringObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, (float)i / 6);
            rectTransform.anchorMax = new Vector2(1, (float)(i + 1) / 6);
            rectTransform.offsetMin = new Vector2(0, 2); // Margen entre cuerdas
            rectTransform.offsetMax = new Vector2(0, -2);

            Image image = stringObj.AddComponent<Image>();
            image.color = normalStringColor;

            stringVisuals[i] = rectTransform;
        }
    }

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

    private IEnumerator AnimateStringColor(Image stringImage)
    {
        Color originalColor = stringImage.color;
        stringImage.color = playedStringColor;

        float elapsed = 0;
        while (elapsed < stringAnimationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / stringAnimationDuration;
            stringImage.color = Color.Lerp(playedStringColor, normalStringColor, t);
            yield return null;
        }

        stringImage.color = originalColor;
    }

    private IEnumerator AnimateStringMovement(RectTransform stringTransform)
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
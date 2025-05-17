using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GuitarNeckUI : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private RectTransform neckArea;
    [SerializeField] private RectTransform[] stringVisuals; // Referencias a los objetos UI de las cuerdas
    private Dictionary<int, Coroutine> stringAnimations = new Dictionary<int, Coroutine>();
    [SerializeField] private Material customStringMaterial; // Asignar en el inspector

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
        else
        {
            // Asegurarse de que cada cuerda tenga una instancia única del material
            InstantiateMaterials();
        }
    }

    private void InstantiateMaterials()
    {
        for (int i = 0; i < stringVisuals.Length; i++)
        {
            Image stringImage = stringVisuals[i].GetComponent<Image>();
            if (stringImage != null && stringImage.material != null)
            {
                // Crear una instancia única del material para cada cuerda
                stringImage.material = new Material(stringImage.material);
            }
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

            // Aplicar material personalizado si existe
            if (customStringMaterial != null)
            {
                image.material = new Material(customStringMaterial);
            }

            stringVisuals[i] = rectTransform;
        }
    }

    public void AnimateString(int stringIndex)
    {
        if (stringIndex < 0 || stringIndex >= stringVisuals.Length) return;

        // Detener cualquier animación existente para esta cuerda
        if (stringAnimations.ContainsKey(stringIndex) && stringAnimations[stringIndex] != null)
        {
            StopCoroutine(stringAnimations[stringIndex]);
        }

        // Cambiar color de la cuerda
        Image stringImage = stringVisuals[stringIndex].GetComponent<Image>();
        if (stringImage != null)
        {
            StartCoroutine(AnimateStringColor(stringImage));
        }

        // Animar movimiento de la cuerda
        stringAnimations[stringIndex] = StartCoroutine(AnimateStringMovement(stringVisuals[stringIndex], stringIndex));
    }

    private IEnumerator AnimateStringColor(Image stringImage, float duration = 0.2f)
    {
        // Guardar el material y el color original
        Material material = stringImage.material;
        Color originalColor = Color.white;


        // Verificar qué propiedad de color está usando el material
        if (material.HasProperty("_Color"))
        {
            originalColor = material.GetColor("_Color");
            material.SetColor("_Color", playedStringColor);
        }
        else if (material.HasProperty("_BaseColor"))
        {
            originalColor = material.GetColor("_BaseColor");
            material.SetColor("_BaseColor", playedStringColor);
        }
        else if (material.HasProperty("_TintColor"))
        {
            originalColor = material.GetColor("_TintColor");
            material.SetColor("_TintColor", playedStringColor);
        }
        else
        {
            // Si no encontramos una propiedad de color conocida, intentamos con el color del Image
            originalColor = stringImage.color;
            stringImage.color = playedStringColor;
        }
        if (material.HasProperty("_EmissionColor"))
        {
            // Color original de emisión (probablemente negro/apagado)
            Color originalEmission = material.GetColor("_EmissionColor");
            // Emisión intensa del color seleccionado
            Color brightEmission = playedStringColor * 2.0f;

            // Activar emisión si no estaba activa
            material.EnableKeyword("_EMISSION");

            // Aplicar emisión
            material.SetColor("_EmissionColor", brightEmission);

            // Animar la emisión de vuelta a cero
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Aplicar la interpolación de color dependiendo de la propiedad disponible
                if (material.HasProperty("_Color"))
                {
                    material.SetColor("_Color", Color.Lerp(playedStringColor, originalColor, t));
                }
                else if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", Color.Lerp(playedStringColor, originalColor, t));
                }
                else if (material.HasProperty("_TintColor"))
                {
                    material.SetColor("_TintColor", Color.Lerp(playedStringColor, originalColor, t));
                }
                else
                {
                    stringImage.color = Color.Lerp(playedStringColor, originalColor, t);
                }

                // Atenuar la emisión
                material.SetColor("_EmissionColor", Color.Lerp(brightEmission, originalEmission, t));

                yield return null;
            }

            // Restaurar emisión original
            material.SetColor("_EmissionColor", originalEmission);

            // Si la emisión estaba apagada originalmente, apagarla de nuevo
            if (originalEmission == Color.black)
            {
                material.DisableKeyword("_EMISSION");
            }

        }
        // Restaurar color original
        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", originalColor);
        }
        else if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", originalColor);
        }
        else if (material.HasProperty("_TintColor"))
        {
            material.SetColor("_TintColor", originalColor);
        }
        else
        {
            stringImage.color = originalColor;
        }
    }


    private IEnumerator AnimateStringMovement(RectTransform stringTransform, int stringIndex)
    {
        // Guardar posición original
        Vector2 originalPosition = stringTransform.anchoredPosition;
        float amplitude = 5f; // Qué tanto se desplaza la cuerda

        try
        {
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
        }
        finally
        {
            // Asegurarse de restaurar la posición original en todos los casos
            stringTransform.anchoredPosition = originalPosition;
            // Limpiar la referencia
            if (stringAnimations.ContainsKey(stringIndex))
            {
                stringAnimations[stringIndex] = null;
            }
        }
    }

    // Añadir a GuitarNeckUI.cs
    public void AnimateStrum(bool upStrum = false)
    {
        // Definir el orden basado en la dirección del rasgueo
        int start = upStrum ? 5 : 0;
        int end = upStrum ? -1 : 6;
        int step = upStrum ? -1 : 1;

        // Tiempo entre cuerdas (debe coincidir con el audio)
        float delayBetweenStrings = 0.03f;

        StartCoroutine(SequentialStrum(start, end, step, delayBetweenStrings));
    }

    private IEnumerator SequentialStrum(int start, int end, int step, float delay)
    {
        // Calcular cuántas cuerdas vamos a animar
        int stringCount = Mathf.Abs(end - start);

        for (int i = 0; i < stringCount; i++)
        {
            int stringIndex = start + (i * step);

            // Asegurarse de que el índice es válido
            if (stringIndex >= 0 && stringIndex < stringVisuals.Length)
            {
                AnimateString(stringIndex);
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
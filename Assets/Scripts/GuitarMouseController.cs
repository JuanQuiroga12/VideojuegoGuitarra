using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GuitarMouseController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private RectTransform guitarNeckArea; // Área de UI que representa el diapasón

    private Vector2 startPosition;
    private List<int> playedStrings = new List<int>();

    public void OnPointerDown(PointerEventData eventData)
    {
        // Posición relativa dentro del área del diapasón (0,0 en esquina inferior izquierda a 1,1 en superior derecha)
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            guitarNeckArea, eventData.position, eventData.pressEventCamera, out localPos);

        // Normalizar posición
        Vector2 normalizedPos = new Vector2(
            (localPos.x - guitarNeckArea.rect.xMin) / guitarNeckArea.rect.width,
            (localPos.y - guitarNeckArea.rect.yMin) / guitarNeckArea.rect.height
        );

        startPosition = normalizedPos;
        playedStrings.Clear();

        // Reproducir cuerda
        int stringIndex = GetStringIndexFromPosition(normalizedPos);
        if (stringIndex >= 0 && audioManager.currentChord >= 0)
        {
            audioManager.PlayString(audioManager.currentChord, stringIndex);
            playedStrings.Add(stringIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            guitarNeckArea, eventData.position, eventData.pressEventCamera, out localPos);

        Vector2 normalizedPos = new Vector2(
            (localPos.x - guitarNeckArea.rect.xMin) / guitarNeckArea.rect.width,
            (localPos.y - guitarNeckArea.rect.yMin) / guitarNeckArea.rect.height
        );

        // Reproducir cuerdas que no se han tocado aún
        int stringIndex = GetStringIndexFromPosition(normalizedPos);
        if (stringIndex >= 0 && !playedStrings.Contains(stringIndex) && audioManager.currentChord >= 0)
        {
            audioManager.PlayString(audioManager.currentChord, stringIndex);
            playedStrings.Add(stringIndex);
        }

        // Detectar rasgueo (movimiento vertical significativo)
        if (Mathf.Abs(normalizedPos.y - startPosition.y) > 0.3f)
        {
            bool upStrum = normalizedPos.y > startPosition.y;
            audioManager.PlayStrum(audioManager.currentChord, upStrum);
            // Reiniciar para evitar múltiples rasgueos
            startPosition = normalizedPos;
            playedStrings.Clear();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reiniciar estado
        playedStrings.Clear();
    }

    private int GetStringIndexFromPosition(Vector2 position)
    {
        // Invertimos Y para que 0 sea la parte inferior (cuerda 6) y 1 sea la parte superior (cuerda 1)
        float invertedY = 1.0f - position.y;

        // Dividimos en 6 secciones iguales
        float stringHeight = 1.0f / 6.0f;

        int stringIndex = Mathf.FloorToInt(invertedY / stringHeight);
        stringIndex = Mathf.Clamp(stringIndex, 0, 5);

        return stringIndex;
    }
}
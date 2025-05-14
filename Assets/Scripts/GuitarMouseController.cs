using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class GuitarMouseController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private RectTransform guitarNeckArea; // �rea UI que representa el diapas�n
    [SerializeField] private GuitarNeckUI neckUI; // Referencia a la UI del diapas�n

    private Vector2 startPosition;
    private List<int> playedStrings = new List<int>();

    private void Start()
    {
        if (guitarNeckArea == null)
        {
            // Si no se asigna un �rea espec�fica, intentar encontrar o crear una
            GameObject areaObj = GameObject.Find("GuitarNeckArea");
            if (areaObj == null)
            {
                areaObj = new GameObject("GuitarNeckArea");
                areaObj.transform.SetParent(transform);
                guitarNeckArea = areaObj.AddComponent<RectTransform>();
                // Configurar tama�o predeterminado
                guitarNeckArea.sizeDelta = new Vector2(300, 500);
            }
            else
            {
                guitarNeckArea = areaObj.GetComponent<RectTransform>();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Posici�n relativa dentro del �rea del diapas�n
        Vector2 localPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            guitarNeckArea, eventData.position, eventData.pressEventCamera, out localPos))
        {
            // Normalizar posici�n
            Vector2 normalizedPos = new Vector2(
                (localPos.x - guitarNeckArea.rect.xMin) / guitarNeckArea.rect.width,
                (localPos.y - guitarNeckArea.rect.yMin) / guitarNeckArea.rect.height
            );

            startPosition = normalizedPos;
            playedStrings.Clear();

            // Reproducir cuerda
            int stringIndex = GetStringIndexFromPosition(normalizedPos);
            if (stringIndex >= 0 && audioManager.CurrentChord >= 0)
            {
                audioManager.PlayString(audioManager.CurrentChord, stringIndex);
                playedStrings.Add(stringIndex);

                // Animar cuerda en la UI
                if (neckUI != null)
                {
                    neckUI.AnimateString(stringIndex);
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            guitarNeckArea, eventData.position, eventData.pressEventCamera, out localPos))
        {
            Vector2 normalizedPos = new Vector2(
                (localPos.x - guitarNeckArea.rect.xMin) / guitarNeckArea.rect.width,
                (localPos.y - guitarNeckArea.rect.yMin) / guitarNeckArea.rect.height
            );

            // Reproducir cuerdas que no se han tocado a�n
            int stringIndex = GetStringIndexFromPosition(normalizedPos);
            if (stringIndex >= 0 && !playedStrings.Contains(stringIndex) && audioManager.CurrentChord >= 0)
            {
                audioManager.PlayString(audioManager.CurrentChord, stringIndex);
                playedStrings.Add(stringIndex);

                // Animar cuerda
                if (neckUI != null)
                {
                    neckUI.AnimateString(stringIndex);
                }
            }

            // Detectar rasgueo (movimiento vertical significativo)
            if (Mathf.Abs(normalizedPos.y - startPosition.y) > 0.3f)
            {
                bool upStrum = normalizedPos.y > startPosition.y;
                audioManager.PlayStrum(audioManager.CurrentChord, upStrum);

                // Animar todas las cuerdas
                if (neckUI != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        neckUI.AnimateString(i);
                    }
                }

                // Reiniciar para evitar m�ltiples rasgueos
                startPosition = normalizedPos;
                playedStrings.Clear();
            }
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
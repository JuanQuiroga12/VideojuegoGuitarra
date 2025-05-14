using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class GuitarTouchpadController : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private GuitarNeckUI neckUI;

    // Referencia al controlador
    private Gamepad gamepad;

    // �reas del touchpad (dividimos en 6 secciones verticales para las cuerdas)
    private readonly float[] stringBoundaries = { 0.0f, 0.167f, 0.333f, 0.5f, 0.667f, 0.833f, 1.0f };

    // Registro de toque actual
    private bool isTouching = false;
    private Vector2 touchStartPosition;
    private Vector2 lastTouchPosition;
    private List<int> playedStrings = new List<int>();

    private void Update()
    {
        // Obtener referencia al gamepad si es necesario
        if (gamepad == null)
        {
            gamepad = Gamepad.current;
            if (gamepad == null) return; // No hay gamepad conectado
        }

        // En un DualSense podr�amos acceder al touchpad directamente
        // Pero dado que estamos simulando, usamos otros controles

        // Simular touchpad con el stick izquierdo y trigger para "tocar"
        bool touchPressed = gamepad.leftTrigger.isPressed;
        Vector2 touchPosition = gamepad.leftStick.ReadValue();

        // Ajustar la posici�n para que vaya de 0 a 1 en ambos ejes
        touchPosition = new Vector2((touchPosition.x + 1) / 2, (touchPosition.y + 1) / 2);

        // Detectar inicio de toque
        if (touchPressed && !isTouching)
        {
            isTouching = true;
            touchStartPosition = touchPosition;
            lastTouchPosition = touchPosition;
            playedStrings.Clear();

            // Reproducir la cuerda tocada
            int stringIndex = GetStringIndexFromPosition(touchPosition);
            if (stringIndex >= 0 && audioManager.CurrentChord >= 0)
            {
                audioManager.PlayString(audioManager.CurrentChord, stringIndex);
                playedStrings.Add(stringIndex);

                // Animar cuerda
                if (neckUI != null)
                {
                    neckUI.AnimateString(stringIndex);
                }

                // Proporcionar retroalimentaci�n h�ptica (vibraci�n)
                ProvideHapticFeedback(stringIndex);
            }
        }
        // Detectar movimiento durante el toque
        else if (touchPressed && isTouching)
        {
            // Detectar si hay un cambio significativo de posici�n
            if (Vector2.Distance(touchPosition, lastTouchPosition) > 0.05f)
            {
                int stringIndex = GetStringIndexFromPosition(touchPosition);
                if (stringIndex >= 0 && !playedStrings.Contains(stringIndex) && audioManager.CurrentChord >= 0)
                {
                    audioManager.PlayString(audioManager.CurrentChord, stringIndex);
                    playedStrings.Add(stringIndex);

                    // Animar cuerda
                    if (neckUI != null)
                    {
                        neckUI.AnimateString(stringIndex);
                    }

                    // Proporcionar retroalimentaci�n h�ptica
                    ProvideHapticFeedback(stringIndex, 0.5f);
                }

                // Detectar rasgueo (movimiento vertical significativo)
                if (Mathf.Abs(touchPosition.y - touchStartPosition.y) > 0.5f)
                {
                    bool upStrum = touchPosition.y > touchStartPosition.y;
                    audioManager.PlayStrum(audioManager.CurrentChord, upStrum);

                    // Animar todas las cuerdas
                    if (neckUI != null)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            neckUI.AnimateString(i);
                        }
                    }

                    // Retroalimentaci�n h�ptica fuerte para rasgueo
                    ProvideHapticFeedback(0, 1.0f);

                    // Reiniciar posici�n de inicio para evitar m�ltiples rasgueos
                    touchStartPosition = touchPosition;
                    playedStrings.Clear();
                }

                lastTouchPosition = touchPosition;
            }
        }
        // Detectar fin de toque
        else if (!touchPressed && isTouching)
        {
            isTouching = false;
        }
    }

    private int GetStringIndexFromPosition(Vector2 position)
    {
        // Invertimos Y para que 0 sea la parte inferior (cuerda 6) y 1 sea la parte superior (cuerda 1)
        float invertedY = 1.0f - position.y;

        // Determinar qu� cuerda corresponde a esta posici�n
        for (int i = 0; i < stringBoundaries.Length - 1; i++)
        {
            if (invertedY >= stringBoundaries[i] && invertedY < stringBoundaries[i + 1])
            {
                return i;
            }
        }

        return -1; // Fuera de rango
    }

    private void ProvideHapticFeedback(int stringIndex, float intensityMultiplier = 1.0f)
    {
        if (gamepad == null) return;

        // Intensidad basada en la cuerda (grave = m�s fuerte)
        float intensity = (0.2f + (5 - stringIndex) * 0.15f) * intensityMultiplier;
        intensity = Mathf.Clamp01(intensity); // Asegurar que est� entre 0 y 1
        float duration = 0.1f;

        // Vibraci�n en ambos motores
        gamepad.SetMotorSpeeds(intensity, intensity);

        // Apagar la vibraci�n despu�s de la duraci�n especificada
        StartCoroutine(StopHapticFeedback(duration));
    }

    private IEnumerator StopHapticFeedback(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }
}
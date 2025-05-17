using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem.DualShock;

public class GuitarTouchpadController : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private GuitarNeckUI neckUI;

    // Áreas para las cuerdas del touchpad (6 secciones verticales)
    private readonly float[] stringBoundaries = { 0.0f, 0.167f, 0.333f, 0.5f, 0.667f, 0.833f, 1.0f };

    // Seguimiento del toque
    private bool isTouching = false;
    private Vector2 touchStartPosition;
    private Vector2 lastTouchPosition;
    private List<int> playedStrings = new List<int>();

    private void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return; // No hay gamepad conectado

        // Para PS4/PS5 con DualShockGamepad o DualSenseGamepad
        if (gamepad is DualShockGamepad dualShock)
        {
            // Simulación usando L2 y el stick izquierdo
            // No podemos acceder directamente a la posición del touchpad en Unity Input System
            bool touchPressed = dualShock.leftTrigger.isPressed;
            Vector2 touchPosition = dualShock.leftStick.ReadValue();

            // Ajustar posición al rango 0-1
            touchPosition = new Vector2((touchPosition.x), (touchPosition.y));

            ProcessTouchInput(touchPressed, touchPosition);
        }
        // Para otros gamepads, usar la misma simulación
        else
        {
            bool touchPressed = gamepad.leftTrigger.isPressed;
            Vector2 touchPosition = gamepad.leftStick.ReadValue();

            // Ajustar posición al rango 0-1
            touchPosition = new Vector2((touchPosition.x + 1) / 2, (touchPosition.y + 1) / 2);

            ProcessTouchInput(touchPressed, touchPosition);
        }
    }

    private void ProcessTouchInput(bool touchPressed, Vector2 touchPosition)
    {
        // Inicio del toque
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

                // Retroalimentación háptica
                ProvideHapticFeedback(stringIndex);
            }
        }
        // Continuación del toque
        else if (touchPressed && isTouching)
        {
            // Detectar cambio significativo de posición
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

                    ProvideHapticFeedback(stringIndex, 0.5f);
                }

                // Detectar rasgueo (movimiento vertical significativo)
                if (Mathf.Abs(touchPosition.y - touchStartPosition.y) > 0.5f)
                {
                    bool upStrum = touchPosition.y > touchStartPosition.y;
                    audioManager.PlayStrum(audioManager.CurrentChord, upStrum);

                    // Animar las cuerdas secuencialmente
                    if (neckUI != null)
                    {
                        neckUI.AnimateStrum(upStrum);
                    }

                    ProvideHapticFeedback(0, 1.0f);
                    touchStartPosition = touchPosition;
                    playedStrings.Clear();
                }

                lastTouchPosition = touchPosition;
            }
        }
        // Fin del toque
        else if (!touchPressed && isTouching)
        {
            isTouching = false;
        }
    }

    private int GetStringIndexFromPosition(Vector2 position)
    {
        // Invertir Y para que 0 sea la parte inferior (cuerda 6) y 1 sea la parte superior (cuerda 1)
        float invertedY = 1.0f - position.y;

        // Determinar qué cuerda corresponde a esta posición
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
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // Intensidad basada en la cuerda (más grave = más fuerte)
        float intensity = (0.2f + (5 - stringIndex) * 0.15f) * intensityMultiplier;
        intensity = Mathf.Clamp01(intensity);
        float duration = 0.1f;

        // Vibración en ambos motores
        gamepad.SetMotorSpeeds(intensity, intensity);

        // Apagar después de la duración
        StartCoroutine(StopHapticFeedback(duration));
    }

    private IEnumerator StopHapticFeedback(float delay)
    {
        yield return new WaitForSeconds(delay);
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }
    }
}
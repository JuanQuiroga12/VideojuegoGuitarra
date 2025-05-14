using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.DualShock;

public class GuitarTouchpadController : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;

    // Referencia al controlador de PS5
    private Gamepad gamepad;

    // Áreas del touchpad (dividimos en 6 secciones verticales para las cuerdas)
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

        // Leer el estado del touchpad
        bool touchPressed = gamepad.leftTrigger.isPressed; // Usamos el trigger como sustituto para "toque"
        Vector2 touchPosition = gamepad.leftStick.ReadValue(); // Usamos el stick como sustituto para "posición"

        // Ajustar la posición para que vaya de 0 a 1 en ambos ejes
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
            if (stringIndex >= 0)
            {
                audioManager.PlayString(audioManager.currentChord, stringIndex);
                playedStrings.Add(stringIndex);
            }
        }
        // Detectar movimiento durante el toque
        else if (touchPressed && isTouching)
        {
            // Detectar si hay un cambio significativo de posición
            if (Vector2.Distance(touchPosition, lastTouchPosition) > 0.05f)
            {
                int stringIndex = GetStringIndexFromPosition(touchPosition);
                if (stringIndex >= 0 && !playedStrings.Contains(stringIndex))
                {
                    audioManager.PlayString(audioManager.currentChord, stringIndex);
                    playedStrings.Add(stringIndex);
                }

                // Detectar rasgueo (movimiento vertical significativo)
                if (Mathf.Abs(touchPosition.y - touchStartPosition.y) > 0.5f)
                {
                    bool upStrum = touchPosition.y > touchStartPosition.y;
                    audioManager.PlayStrum(audioManager.currentChord, upStrum);
                    // Reiniciar posición de inicio para evitar múltiples rasgueos
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

    // Añade esto a tu GuitarTouchpadController
    private void ProvideHapticFeedback(int stringIndex)
    {
        if (gamepad == null || !(gamepad is DualSenseGamepadHID)) return;

        DualSenseGamepadHID dualSense = (DualSenseGamepadHID)gamepad;

        // Intensidad basada en la cuerda (grave = más fuerte)
        float intensity = 0.2f + (5 - stringIndex) * 0.15f;
        float duration = 0.1f;

        // Vibración en ambos motores
        dualSense.SetMotorSpeeds(intensity, intensity);

        // Apagar la vibración después de la duración especificada
        StartCoroutine(StopHapticFeedback(dualSense, duration));
    }

    private System.Collections.IEnumerator StopHapticFeedback(DualSenseGamepadHID dualSense, float delay)
    {
        yield return new WaitForSeconds(delay);
        dualSense.SetMotorSpeeds(0, 0);
    }

}
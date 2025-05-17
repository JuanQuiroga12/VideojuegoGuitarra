// GuitarKeyboardController.cs    (ponlo en Assets/Scripts)
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GuitarAudioManager))]
public class GuitarKeyboardController : MonoBehaviour, GuitarControls.IGuitarActions
{
    private GuitarControls controls;
    private GuitarAudioManager audio;
    [SerializeField] private GuitarNeckUI neckUI;

    // Umbral para considerar que el joystick está inclinado en una dirección
    [SerializeField] private float stickThreshold = 0.3f;

    private void Awake()
    {
        audio = GetComponent<GuitarAudioManager>();
        controls = new GuitarControls();
        controls.Guitar.SetCallbacks(this);

        // Si no se asigna en el editor, buscar en la escena
        if (neckUI == null)
            neckUI = FindObjectOfType<GuitarNeckUI>();
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    /* -------- IGuitarActions -------- */
    public void OnString1(InputAction.CallbackContext ctx) {
        if (ctx.performed)
        {
            Play(0);
            // Añadir animación
            if (neckUI != null)
                neckUI.AnimateString(0);
        }
    }
    public void OnString2(InputAction.CallbackContext ctx) {
        if (ctx.performed)
        {
            Play(1);
            if (neckUI != null)
                neckUI.AnimateString(1);
        }
    }
    public void OnString3(InputAction.CallbackContext ctx) {
        if (ctx.performed)
        {
            Play(2);
            if (neckUI != null)
                neckUI.AnimateString(2);
        }
    }
    public void OnString4(InputAction.CallbackContext ctx) {
        if (ctx.performed)
        {
            Play(3);
            if (neckUI != null)
                neckUI.AnimateString(3);
        }
    }
    public void OnString5(InputAction.CallbackContext ctx) {
        if (ctx.performed)
        {
            Play(4);
            if (neckUI != null)
                neckUI.AnimateString(4);
        }
    }
    public void OnString6(InputAction.CallbackContext ctx) {
        if (ctx.performed)
        {
            Play(5);
            if (neckUI != null)
                neckUI.AnimateString(5);
        }
    }
    // Método modificado para detectar la dirección del rasgueo
    public void OnStrum(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && audio.CurrentChord >= 0)
        {
            // Determinar la dirección del rasgueo basado en el joystick izquierdo
            bool upStrum = false;

            if (Gamepad.current != null)
            {
                // Leer la posición Y del joystick izquierdo
                float leftStickY = Gamepad.current.leftStick.y.ReadValue();

                // Si el joystick está hacia arriba (positivo), hacer un rasgueo hacia arriba
                // Si el joystick está hacia abajo (negativo) o neutral, hacer un rasgueo hacia abajo (por defecto)
                upStrum = leftStickY > stickThreshold;

                Debug.Log($"Rasgueo direccional: {(upStrum ? "Arriba" : "Abajo")}, Valor joystick: {leftStickY}");
            }

            audio.PlayStrum(audio.CurrentChord, upStrum);

            // Animar las cuerdas secuencialmente
            if (neckUI != null)
            {
                neckUI.AnimateStrum(upStrum);
            }
        }
    }

    /* --- las demás callbacks del mapa no las usamos aquí --- */
    public void OnChord_A(InputAction.CallbackContext c) { }
    public void OnChord_B(InputAction.CallbackContext c) { }
    public void OnChord_C(InputAction.CallbackContext c) { }
    public void OnChord_D(InputAction.CallbackContext c) { }
    public void OnChord_E(InputAction.CallbackContext c) { }
    public void OnChord_F(InputAction.CallbackContext c) { }
    public void OnNextPage(InputAction.CallbackContext c) { }
    public void OnPrevPage(InputAction.CallbackContext c) { }
    public void OnTouchPosition(InputAction.CallbackContext c) { }
    public void OnTouchContact(InputAction.CallbackContext c) { }

    public void OnRightStickY(InputAction.CallbackContext c) { }

    public void OnRightStickX(InputAction.CallbackContext c) { }

    /* -------------- helpers -------------- */
    private void Play(int stringIndex)
    {
        if (audio.CurrentChord < 0) { Debug.Log("Ningún acorde seleccionado"); return; }
        audio.PlayString(audio.CurrentChord, stringIndex);
    }
}

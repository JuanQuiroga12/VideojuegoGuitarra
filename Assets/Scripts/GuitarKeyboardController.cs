// GuitarKeyboardController.cs    (ponlo en Assets/Scripts)
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GuitarAudioManager))]
public class GuitarKeyboardController : MonoBehaviour, GuitarControls.IGuitarActions
{
    private GuitarControls controls;
    private GuitarAudioManager audio;

    private void Awake()
    {
        audio = GetComponent<GuitarAudioManager>();
        controls = new GuitarControls();
        controls.Guitar.SetCallbacks(this);
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    /* -------- IGuitarActions -------- */
    public void OnString1(InputAction.CallbackContext ctx) { if (ctx.performed) Play(0); }
    public void OnString2(InputAction.CallbackContext ctx) { if (ctx.performed) Play(1); }
    public void OnString3(InputAction.CallbackContext ctx) { if (ctx.performed) Play(2); }
    public void OnString4(InputAction.CallbackContext ctx) { if (ctx.performed) Play(3); }
    public void OnString5(InputAction.CallbackContext ctx) { if (ctx.performed) Play(4); }
    public void OnString6(InputAction.CallbackContext ctx) { if (ctx.performed) Play(5); }
    public void OnStrum(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && audio.CurrentChord >= 0)
            audio.PlayStrum(audio.CurrentChord, false);
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

    /* -------------- helpers -------------- */
    private void Play(int stringIndex)
    {
        if (audio.CurrentChord < 0) { Debug.Log("Ningún acorde seleccionado"); return; }
        audio.PlayString(audio.CurrentChord, stringIndex);
    }
}

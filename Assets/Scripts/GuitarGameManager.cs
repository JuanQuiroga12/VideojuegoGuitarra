using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class GuitarGameManager : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private RadialMenuController menuController;
    [SerializeField] private GuitarNeckUI neckUI;

    // Sistemas de entrada
    [SerializeField] private GuitarTouchpadController touchpadController;
    [SerializeField] private GuitarMouseController mouseController;

    // Referencia a los controles
    private GuitarControls controls;

    private void Awake()
    {
        controls = new GuitarControls();

        // Configurar controlador según dispositivo
        if (Gamepad.current != null && Gamepad.current is DualSenseGamepadHID)
        {
            touchpadController.gameObject.SetActive(true);
            mouseController.gameObject.SetActive(false);
            Debug.Log("PS5 DualSense controller detected - using touchpad controls");
        }
        else
        {
            touchpadController.gameObject.SetActive(false);
            mouseController.gameObject.SetActive(true);
            Debug.Log("No PS5 controller detected - using mouse controls");
        }
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        // Detectar cambios de dispositivo de entrada
        if (Gamepad.current != null && Gamepad.current is DualSenseGamepadHID &&
            !touchpadController.gameObject.activeSelf)
        {
            touchpadController.gameObject.SetActive(true);
            mouseController.gameObject.SetActive(false);
            Debug.Log("PS5 DualSense controller connected - switching to touchpad controls");
        }
        else if ((Gamepad.current == null || !(Gamepad.current is DualSenseGamepadHID)) &&
                !mouseController.gameObject.activeSelf)
        {
            touchpadController.gameObject.SetActive(false);
            mouseController.gameObject.SetActive(true);
            Debug.Log("PS5 controller disconnected - switching to mouse controls");
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

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
        if (Gamepad.current != null)
        {
            touchpadController.gameObject.SetActive(true);
            mouseController.gameObject.SetActive(false);
            Debug.Log("Gamepad controller detected - using touchpad controls");
        }
        else
        {
            touchpadController.gameObject.SetActive(false);
            mouseController.gameObject.SetActive(true);
            Debug.Log("No gamepad detected - using mouse controls");
        }

        UpdateControllerMode();

    }

    private void OnEnable()
    {
        controls.Enable();
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    private void OnDisable()
    {
        controls.Disable();
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added || change == InputDeviceChange.Removed)
        {
            UpdateControllerMode();
        }
    }

    private void UpdateControllerMode()
    {
        if (Gamepad.current != null)
        {
            touchpadController.gameObject.SetActive(true);
            mouseController.gameObject.SetActive(false);
            Debug.Log("Gamepad controller detected - using touchpad controls");
        }
        else
        {
            touchpadController.gameObject.SetActive(false);
            mouseController.gameObject.SetActive(true);
            Debug.Log("No gamepad detected - using mouse controls");
        }
    }
}
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
        if (Gamepad.current != null && !touchpadController.gameObject.activeSelf)
        {
            touchpadController.gameObject.SetActive(true);
            mouseController.gameObject.SetActive(false);
            Debug.Log("Gamepad controller connected - switching to touchpad controls");
        }
        else if (Gamepad.current == null && !mouseController.gameObject.activeSelf)
        {
            touchpadController.gameObject.SetActive(false);
            mouseController.gameObject.SetActive(true);
            Debug.Log("Gamepad disconnected - switching to mouse controls");
        }
    }
}
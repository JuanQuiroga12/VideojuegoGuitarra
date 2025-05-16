using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private float radius = 200f;
    [SerializeField] private bool startMajorPage = true;
    [SerializeField] private float joystickThreshold = 0.5f;

    private Button[] chordButtons;
    private int currentPage = 0;
    private int selectedChord = -1;
    private GuitarControls controls;

    // Posiciones de los botones en el círculo (en grados, 0 = arriba)
    // Ajusta estos valores según la posición real de tus botones
    private readonly float[] buttonAngles = { 0f, 60f, 120f, 180f, 240f, 300f };
    // Índices de acordes correspondientes a cada posición
    // [A, E, Bm, F#m, C#m, D] según la imagen
    private readonly int[] angleToChordMap = { 0,1,2,3,4,5 };

    private void Awake()
    {
        controls = new GuitarControls();

        // Configuraciones originales para teclado
        controls.Guitar.Chord_A.performed += _ => SelectChord(0);
        controls.Guitar.Chord_B.performed += _ => SelectChord(1);
        controls.Guitar.Chord_C.performed += _ => SelectChord(2);
        controls.Guitar.Chord_D.performed += _ => SelectChord(3);
        controls.Guitar.Chord_E.performed += _ => SelectChord(4);
        controls.Guitar.Chord_F.performed += _ => SelectChord(5);

        controls.Guitar.NextPage.performed += ctx => ChangePage(true);
        controls.Guitar.PrevPage.performed += ctx => ChangePage(false);

        chordButtons = GetComponentsInChildren<Button>();
        PositionButtons();

        SetPage(startMajorPage ? 0 : 1);
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
        // Obtener entrada del joystick derecho para la selección de acordes
        if (Gamepad.current != null)
        {
            Vector2 rightStick = Gamepad.current.rightStick.ReadValue();

            // Solo procesar si el stick se mueve más allá del umbral
            if (rightStick.magnitude > joystickThreshold)
            {
                // Convertir dirección del joystick a ángulo
                float angle = Mathf.Atan2(rightStick.y, rightStick.x) * Mathf.Rad2Deg;
                // Ajustar para que 0 sea hacia arriba (por defecto, 0 es hacia la derecha)
                angle = (90 - angle) % 360;
                if (angle < 0) angle += 360f;

                // Encontrar el botón más cercano al ángulo
                int closestButtonIndex = 0;
                float closestAngleDiff = 360f;

                for (int i = 0; i < buttonAngles.Length; i++)
                {
                    float angleDiff = Mathf.Abs(Mathf.DeltaAngle(angle, buttonAngles[i]));
                    if (angleDiff < closestAngleDiff)
                    {
                        closestAngleDiff = angleDiff;
                        closestButtonIndex = i;
                    }
                }

                // Seleccionar el acorde correspondiente a la posición
                int chordIndex = angleToChordMap[closestButtonIndex];
                if (chordIndex != selectedChord)
                {
                    SelectChord(chordIndex);
                    Debug.Log($"Ángulo: {angle}, Botón más cercano: {closestButtonIndex}, Acorde: {chordIndex}");
                }
            }
        }
    }

    private void PositionButtons()
    {
        if (chordButtons.Length != 6) { Debug.LogError("Necesitas 6 botones"); return; }

        for (int i = 0; i < 6; i++)
        {
            float rad = Mathf.Deg2Rad * buttonAngles[i];
            chordButtons[angleToChordMap[i]].GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius;
        }
    }

    private void SelectChord(int index)
    {
        selectedChord = index;
        audioManager.SetCurrentChord(index);
        Debug.Log($"Seleccionando acorde: {index}");

        // Retroalimentación visual
        for (int i = 0; i < chordButtons.Length; i++)
        {
            ColorBlock colors = chordButtons[i].colors;
            colors.normalColor = (i == index) ? Color.yellow : Color.white;
            chordButtons[i].colors = colors;
        }
    }

    private void ChangePage(bool next)
    {
        int newPage = next ? (currentPage + 1) % audioManager.chordPages.Length :
                            (currentPage - 1 + audioManager.chordPages.Length) % audioManager.chordPages.Length;
        SetPage(newPage);
    }

    private void SetPage(int pageIndex)
    {
        currentPage = pageIndex;
        audioManager.SetCurrentPage(pageIndex);
        selectedChord = -1;

        // Actualizar etiquetas de botones
        for (int i = 0; i < chordButtons.Length && i < audioManager.chordPages[currentPage].chords.Length; i++)
        {
            Text buttonText = chordButtons[i].GetComponentInChildren<Text>();
            TMPro.TextMeshProUGUI tmpText = chordButtons[i].GetComponentInChildren<TMPro.TextMeshProUGUI>();

            string chordName = audioManager.chordPages[currentPage].chords[i].chordName;

            if (buttonText != null)
            {
                buttonText.text = chordName;
            }
            else if (tmpText != null)
            {
                tmpText.text = chordName;
            }
        }

        audioManager.SetCurrentChord(-1);
    }
}
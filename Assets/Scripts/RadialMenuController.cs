using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RadialMenuController : MonoBehaviour
{
    [SerializeField] private GuitarAudioManager audioManager;
    [SerializeField] private float radius = 200f;
    [SerializeField] private bool startMajorPage = true;

    private Button[] chordButtons;
    private int currentPage = 0;

    // Referencia al sistema de input
    private GuitarControls controls;

    private void Awake()
    {
        // Inicializar controles
        controls = new GuitarControls();

        // Configurar eventos de input para los acordes
        controls.Guitar.Chord_A.performed += ctx => SelectChord(0);
        controls.Guitar.Chord_B.performed += ctx => SelectChord(1);
        controls.Guitar.Chord_C.performed += ctx => SelectChord(2);
        controls.Guitar.Chord_D.performed += ctx => SelectChord(3);
        controls.Guitar.Chord_E.performed += ctx => SelectChord(4);
        controls.Guitar.Chord_F.performed += ctx => SelectChord(5);
        controls.Guitar.Chord_G.performed += ctx => SelectChord(6);

        // Eventos para cambiar de página
        controls.Guitar.NextPage.performed += ctx => ChangePage(true);
        controls.Guitar.PrevPage.performed += ctx => ChangePage(false);

        // Obtener botones del menú radial
        chordButtons = GetComponentsInChildren<Button>();
        PositionButtons();

        // Inicializar en la primera página
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

    private void PositionButtons()
    {
        if (chordButtons.Length < 7) return; // Necesitamos al menos 7 botones

        float angleStep = 2 * Mathf.PI / 7;

        for (int i = 0; i < 7; i++)
        {
            float angle = i * angleStep;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);

            chordButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // Configurar evento de clic
            int index = i;
            chordButtons[i].onClick.AddListener(() => SelectChord(index));
        }
    }

    private void SelectChord(int index)
    {
        audioManager.SetCurrentChord(index);

        // Visual feedback (highlight selected button)
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

        // Actualizar etiquetas de botones según la página
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

        // Resetear selección de acorde
        audioManager.SetCurrentChord(-1);
    }
}
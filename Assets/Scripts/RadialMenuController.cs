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
        controls.Guitar.Chord_A.performed += _ => SelectChord(0);
        controls.Guitar.Chord_B.performed += _ => SelectChord(1);
        controls.Guitar.Chord_C.performed += _ => SelectChord(2);
        controls.Guitar.Chord_D.performed += _ => SelectChord(3);
        controls.Guitar.Chord_E.performed += _ => SelectChord(4);
        controls.Guitar.Chord_F.performed += _ => SelectChord(5);

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
        if (chordButtons.Length != 6) { Debug.LogError("Necesitas 6 botones"); return; }
        float angleStep = 360f / 6f;
        for (int i = 0; i < 6; i++)
        {
            float rad = Mathf.Deg2Rad * (90 - i * angleStep);
            chordButtons[i].GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * radius;
        }
    }

    private void SelectChord(int index)
    {
        audioManager.SetCurrentChord(index);
        Debug.Log($"Seleccionando acorde: {index}");
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
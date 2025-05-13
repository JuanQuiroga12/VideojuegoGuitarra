using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RadialMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public GuitarController guitarController;

    [Header("Configuración de Menú Radial")]
    public float radius = 200f;
    public int startPageIndex = 0;  // Página inicial (0-4)

    private List<Button> chordButtons = new List<Button>();
    private int currentPageIndex;

    void Start()
    {
        if (guitarController == null)
        {
            Debug.LogError("RadialMenuController: Asigna el GuitarController.");
            enabled = false;
            return;
        }

        foreach (Transform child in transform)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
                chordButtons.Add(btn);
        }

        if (chordButtons.Count == 0)
        {
            Debug.LogError("No hay botones hijos en RadialMenu.");
            enabled = false;
            return;
        }

        currentPageIndex = startPageIndex;
        UpdateButtons();
        DistributeButtons();
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame)
                PrevPage();
            if (Keyboard.current.rKey.wasPressedThisFrame)
                NextPage();

            // Ejemplo: teclas 1-6 para disparar acordes
            if (Keyboard.current.digit1Key.wasPressedThisFrame) OnChordTriggered(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) OnChordTriggered(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) OnChordTriggered(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) OnChordTriggered(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) OnChordTriggered(4);
            if (Keyboard.current.digit6Key.wasPressedThisFrame) OnChordTriggered(5);
        }
    }

    private void OnChordTriggered(int index)
    {
        guitarController.PlayChord(currentPageIndex, index);
        if (index < chordButtons.Count)
            StartCoroutine(AnimateButtonPress(chordButtons[index]));
    }

    private void DistributeButtons()
    {
        int numButtons = chordButtons.Count;
        float angleStep = 360f / numButtons;
        float startAngle = 90f;

        for (int i = 0; i < numButtons; i++)
        {
            RectTransform btnRect = chordButtons[i].GetComponent<RectTransform>();
            if (btnRect != null)
            {
                btnRect.anchorMin = btnRect.anchorMax = new Vector2(0.5f, 0.5f);
                btnRect.pivot = new Vector2(0.5f, 0.5f);
                float angleDeg = startAngle - (angleStep * i);
                float rad = angleDeg * Mathf.Deg2Rad;
                float x = Mathf.Cos(rad) * radius;
                float y = Mathf.Sin(rad) * radius;
                btnRect.anchoredPosition = new Vector2(x, y);
            }
            chordButtons[i].onClick.RemoveAllListeners();
            int index = i;
            chordButtons[i].onClick.AddListener(() => OnChordTriggered(index));
        }
    }

    private void UpdateButtons()
    {
        string[] chordNames = guitarController.GetChordNames(currentPageIndex);
        for (int i = 0; i < chordButtons.Count; i++)
        {
            TMP_Text textComp = chordButtons[i].GetComponentInChildren<TMP_Text>();
            if (textComp != null && i < chordNames.Length)
                textComp.text = chordNames[i];
        }
        Debug.Log($"Página actual: {currentPageIndex + 1}");
    }

    private IEnumerator AnimateButtonPress(Button chordButton)
    {
        Transform btnTransform = chordButton.transform;
        Vector3 originalScale = btnTransform.localScale;
        Vector3 pressedScale = originalScale * 1.1f;
        btnTransform.localScale = pressedScale;
        yield return new WaitForSeconds(0.1f);
        btnTransform.localScale = originalScale;
    }

    private void NextPage()
    {
        currentPageIndex = (currentPageIndex + 1) % 5;  // Ciclo entre 5 páginas
        UpdateButtons();
    }

    private void PrevPage()
    {
        currentPageIndex = (currentPageIndex + 4) % 5;  // Ciclo entre 5 páginas hacia atrás
        UpdateButtons();
    }
}

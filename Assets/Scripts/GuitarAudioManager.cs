using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class ChordStrings
{
    public string chordName;
    public AudioClip[] stringClips = new AudioClip[6]; // 6 cuerdas para cada acorde
}

[System.Serializable]
public class ChordPage
{
    public ChordStrings[] chords = new ChordStrings[6]; // 6 acordes por página
}

public class GuitarAudioManager : MonoBehaviour
{
    public ChordPage[] chordPages = new ChordPage[5]; // 5 páginas de acordes

    [SerializeField] private AudioSource[] stringSources; // Array de AudioSources para reproducir múltiples cuerdas

    private int currentPage = 0;
    private int _currentChord = -1;

    // Propiedad para acceder al acorde actual desde otros scripts
    public int CurrentChord
    {
        get { return _currentChord; }
        private set { _currentChord = value; }
    }

    private void Awake()
    {
        // Inicializar AudioSources si no están configurados
        if (stringSources == null || stringSources.Length == 0)
        {
            stringSources = new AudioSource[6];
            for (int i = 0; i < 6; i++)
            {
                GameObject sourceObj = new GameObject($"StringSource_{i}");
                sourceObj.transform.parent = transform;
                stringSources[i] = sourceObj.AddComponent<AudioSource>();
            }
        }

        // Inicializar estructura de datos si es necesario
        InitializeChordStructure();
    }

    private void InitializeChordStructure()
    {
        // Crear la estructura de acordes si no está inicializada
        if (chordPages == null || chordPages.Length == 0)
        {
            chordPages = new ChordPage[5];

            for (int p = 0; p < 5; p++)
            {
                chordPages[p] = new ChordPage();
                chordPages[p].chords = new ChordStrings[6];

                for (int c = 0; c < 6; c++)
                {
                    chordPages[p].chords[c] = new ChordStrings();
                    chordPages[p].chords[c].stringClips = new AudioClip[6];
                }
            }
        }
    }

    // Método para reproducir una cuerda específica de un acorde específico
    public void PlayString(int chordIndex, int stringIndex)
    {
        if (chordIndex >= 0 && chordIndex < chordPages[currentPage].chords.Length &&
            stringIndex >= 0 && stringIndex < 6)
        {
            AudioClip clip = chordPages[currentPage].chords[chordIndex].stringClips[stringIndex];
            if (clip != null)
            {
                stringSources[stringIndex].clip = clip;
                stringSources[stringIndex].Play();
            }
        }
    }

    // Método para simular un rasgueo completo 
    public void PlayStrum(int chordIndex, bool upStrum = false)
    {
        if (chordIndex >= 0 && chordIndex < chordPages[currentPage].chords.Length)
        {
            // Determina el orden de cuerdas según sea rasgueo hacia arriba o abajo
            int start = upStrum ? 5 : 0;
            int end = upStrum ? -1 : 6;
            int step = upStrum ? -1 : 1;

            // Añade un pequeño retraso entre cuerdas
            float delayBetweenStrings = 0.03f;

            for (int i = 0; i < 6; i++)
            {
                int stringIndex = upStrum ? 5 - i : i;
                StartCoroutine(PlayDelayedString(chordIndex, stringIndex, i * delayBetweenStrings));
            }
        }
    }

    private IEnumerator PlayDelayedString(int chordIndex, int stringIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayString(chordIndex, stringIndex);
    }

    // Método para cambiar la página actual
    public void SetCurrentPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < chordPages.Length)
        {
            currentPage = pageIndex;
        }
    }

    // Método para seleccionar un acorde actual
    public void SetCurrentChord(int chordIndex)
    {
        CurrentChord = chordIndex;
    }

    // Método para obtener el nombre del acorde actual
    public string GetCurrentChordName()
    {
        if (CurrentChord >= 0 && CurrentChord < chordPages[currentPage].chords.Length)
        {
            return chordPages[currentPage].chords[CurrentChord].chordName;
        }
        return "";
    }
}
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

    // Modifica este método en GuitarAudioManager.cs para depurar
    private void Awake()
    {
        // Añade debug para verificar
        Debug.Log("Inicializando AudioManager");

        // Inicializar AudioSources si no están configurados
        if (stringSources == null || stringSources.Length == 0)
        {
            Debug.Log("Creando nuevos AudioSources");
            stringSources = new AudioSource[6];
            for (int i = 0; i < 6; i++)
            {
                GameObject sourceObj = new GameObject($"StringSource_{i}");
                sourceObj.transform.parent = transform;
                stringSources[i] = sourceObj.AddComponent<AudioSource>();
                Debug.Log($"AudioSource {i} creado: {stringSources[i] != null}");
            }
        }
        else
        {
            Debug.Log($"AudioSources existentes: {stringSources.Length}");
            for (int i = 0; i < stringSources.Length; i++)
            {
                Debug.Log($"AudioSource {i}: {stringSources[i] != null}");
            }
        }

        InitializeChordStructure();
    }

    // Añade esta función a GuitarAudioManager.cs
    public void TestAudio()
    {
        Debug.Log($"Test Audio: Página actual: {currentPage}, Acorde actual: {CurrentChord}");

        // Probar todos los audios en la página actual
        for (int c = 0; c < chordPages[currentPage].chords.Length; c++)
        {
            Debug.Log($"Probando acorde {c}: {chordPages[currentPage].chords[c].chordName}");
            for (int s = 0; s < 6; s++)
            {
                AudioClip clip = chordPages[currentPage].chords[c].stringClips[s];
                Debug.Log($"  - Cuerda {s}: Clip {clip != null}");
                if (clip != null && stringSources[s] != null)
                {
                    stringSources[s].PlayOneShot(clip);
                    // Esperar antes de reproducir el siguiente
                    System.Threading.Thread.Sleep(500);
                }
            }
        }
    }

    // Añade esta función a GuitarAudioManager para depurar
    public void VerifyAudioClips()
    {
        Debug.Log("Verificando clips de audio");
        for (int p = 0; p < chordPages.Length; p++)
        {
            Debug.Log($"Página {p}:");
            for (int c = 0; c < chordPages[p].chords.Length; c++)
            {
                string chordName = chordPages[p].chords[c].chordName;
                Debug.Log($"  Acorde {c}: {chordName}");
                for (int s = 0; s < 6; s++)
                {
                    AudioClip clip = chordPages[p].chords[c].stringClips[s];
                    Debug.Log($"    - Cuerda {s}: {(clip != null ? clip.name : "NULL")}");
                }
            }
        }
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
        Debug.Log($"Intentando reproducir cuerda {stringIndex} del acorde {chordIndex}");

        if (chordIndex >= 0 && chordIndex < chordPages[currentPage].chords.Length &&
            stringIndex >= 0 && stringIndex < 6)
        {
            AudioClip clip = chordPages[currentPage].chords[chordIndex].stringClips[stringIndex];

            if (clip != null)
            {
                Debug.Log($"Clip encontrado: {clip.name}");
                if (stringSources[stringIndex] != null)
                {
                    Debug.Log($"AudioSource OK: {stringSources[stringIndex].name}");
                    stringSources[stringIndex].clip = clip;
                    stringSources[stringIndex].Play();
                }
                else
                {
                    Debug.LogError($"AudioSource en posición {stringIndex} es null");
                }
            }
            else
            {
                Debug.LogError($"No hay clip asignado para cuerda {stringIndex} del acorde {chordIndex}");
            }
        }
        else
        {
            Debug.LogError($"Índices fuera de rango: chord={chordIndex}, string={stringIndex}");
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
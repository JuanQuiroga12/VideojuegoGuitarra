using UnityEngine;
using System.Collections.Generic;

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

    [SerializeField] private AudioSource[] stringSources; // 6 AudioSources para permitir múltiples cuerdas a la vez

    private int currentPage = 0;
    private int currentChord = -1;

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

    public void PlayStrum(int chordIndex, bool upStrum = false)
    {
        if (chordIndex >= 0 && chordIndex < chordPages[currentPage].chords.Length)
        {
            // Determina el orden de las cuerdas según sea rasgueo hacia arriba o abajo
            int start = upStrum ? 5 : 0;
            int end = upStrum ? -1 : 6;
            int step = upStrum ? -1 : 1;

            // Añade un pequeño retraso entre cuerdas para simular un rasgueo
            float delayBetweenStrings = 0.03f;

            for (int i = 0; i < 6; i++)
            {
                int stringIndex = upStrum ? 5 - i : i;
                StartCoroutine(PlayDelayedString(chordIndex, stringIndex, i * delayBetweenStrings));
            }
        }
    }

    private System.Collections.IEnumerator PlayDelayedString(int chordIndex, int stringIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayString(chordIndex, stringIndex);
    }

    public void SetCurrentPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < chordPages.Length)
        {
            currentPage = pageIndex;
        }
    }

    public void SetCurrentChord(int chordIndex)
    {
        currentChord = chordIndex;
    }

    // Añade esto a tu GuitarAudioManager
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private int poolSize = 12; // 6 cuerdas * 2 para tener rotación

    private void Start()
    {
        // Inicializar pool
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            audioSourcePool.Enqueue(source);
        }
    }

    private AudioSource GetAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }

        // Si no hay fuentes disponibles, crear una nueva
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = false;
        return source;
    }

    private void RecycleAudioSource(AudioSource source)
    {
        audioSourcePool.Enqueue(source);
    }

}
using UnityEngine;

public class GuitarController : MonoBehaviour
{
    [Header("Página 1 - Acordes")]
    public string[] page1ChordNames = { "A", "D", "E", "C#m", "Bm", "F#m" };
    public AudioClip[] page1ChordClips;

    [Header("Página 2 - Acordes")]
    public string[] page2ChordNames = { "B", "E", "F#", "D#m", "C#m", "G#m" };
    public AudioClip[] page2ChordClips;

    [Header("Página 3 - Acordes")]
    public string[] page3ChordNames = { "C", "F", "G", "Em", "Dm", "Am" };
    public AudioClip[] page3ChordClips;

    [Header("Página 4 - Acordes")]
    public string[] page4ChordNames = { "Db", "Gb", "Ab", "Fm", "Ebm", "Bbm" };
    public AudioClip[] page4ChordClips;

    [Header("Página 5 - Acordes")]
    public string[] page5ChordNames = { "Eb", "Ab", "Bb", "Gm", "Fm", "Cm" };
    public AudioClip[] page5ChordClips;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Reproduce un acorde según la página actual y el índice del acorde
    public void PlayChord(int pageIndex, int chordIndex)
    {
        AudioClip clip = null;

        switch (pageIndex)
        {
            case 0: clip = page1ChordClips[chordIndex]; break;
            case 1: clip = page2ChordClips[chordIndex]; break;
            case 2: clip = page3ChordClips[chordIndex]; break;
            case 3: clip = page4ChordClips[chordIndex]; break;
            case 4: clip = page5ChordClips[chordIndex]; break;
        }

        if (clip != null)
            audioSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"GuitarController: No AudioClip asignado en la página {pageIndex + 1}, acorde {chordIndex}.");
    }

    // Devuelve los nombres de los acordes según la página actual
    public string[] GetChordNames(int pageIndex)
    {
        switch (pageIndex)
        {
            case 0: return page1ChordNames;
            case 1: return page2ChordNames;
            case 2: return page3ChordNames;
            case 3: return page4ChordNames;
            case 4: return page5ChordNames;
            default: return new string[0];
        }
    }
}

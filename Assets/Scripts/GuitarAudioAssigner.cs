using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class GuitarAudioAssigner : EditorWindow
{
    private GuitarAudioManager targetManager;
    private string audioRootPath = "Assets/Audio";

    [MenuItem("Tools/Guitar Audio Assigner")]
    public static void ShowWindow()
    {
        GetWindow<GuitarAudioAssigner>("Guitar Audio Assigner");
    }

    private void OnGUI()
    {
        targetManager = (GuitarAudioManager)EditorGUILayout.ObjectField("Audio Manager", targetManager, typeof(GuitarAudioManager), true);
        audioRootPath = EditorGUILayout.TextField("Audio Root Path", audioRootPath);

        if (GUILayout.Button("Auto-Assign Audio Clips") && targetManager != null)
        {
            AssignAudioClips();
        }
    }

    private void AssignAudioClips()
    {
        // Inicializar estructura si es necesario
        if (targetManager.chordPages == null || targetManager.chordPages.Length != 5)
        {
            targetManager.chordPages = new ChordPage[5];
            for (int p = 0; p < 5; p++)
            {
                targetManager.chordPages[p] = new ChordPage();
                targetManager.chordPages[p].chords = new ChordStrings[6];
                for (int c = 0; c < 6; c++)
                {
                    targetManager.chordPages[p].chords[c] = new ChordStrings();
                    targetManager.chordPages[p].chords[c].stringClips = new AudioClip[6];
                }
            }
        }

        // Para cada página
        for (int pageIndex = 0; pageIndex < 5; pageIndex++)
        {
            string pagePath = Path.Combine(audioRootPath, $"Page{pageIndex + 1}");

            // Para cada acorde en la página
            for (int chordIndex = 0; chordIndex < 6; chordIndex++)
            {
                // Nombres comunes de acordes según la página
                string[] chordNames = {
                    "A", "B", "C", "D", "E", "F", "G",
                    "Am", "Bm", "Cm", "Dm", "Em", "Fm", "Gm",
                    "A7", "B7", "C7", "D7", "E7", "F7", "G7",
                    "Amaj7", "Bmaj7", "Cmaj7", "Dmaj7", "Emaj7", "Fmaj7", "Gmaj7",
                    "Asus4", "Bsus4", "Csus4", "Dsus4", "Esus4", "Fsus4", "Gsus4"
                };

                string chordName = chordNames[pageIndex * 6 + chordIndex];
                targetManager.chordPages[pageIndex].chords[chordIndex].chordName = chordName;

                string chordPath = Path.Combine(pagePath, chordName);

                // Para cada cuerda del acorde
                for (int stringIndex = 0; stringIndex < 6; stringIndex++)
                {
                    string filePath = Path.Combine(chordPath, $"String{stringIndex + 1}.wav");
                    AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(filePath);

                    if (clip != null)
                    {
                        targetManager.chordPages[pageIndex].chords[chordIndex].stringClips[stringIndex] = clip;
                    }
                    else
                    {
                        Debug.LogWarning($"No audio clip found at: {filePath}");
                    }
                }
            }
        }

        EditorUtility.SetDirty(targetManager);
        AssetDatabase.SaveAssets();
        Debug.Log("Audio clips assigned successfully!");
    }
}
#endif
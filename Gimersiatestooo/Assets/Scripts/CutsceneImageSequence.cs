using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CutsceneImageSequence : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Image[] images;              // urutan gambar cutscene
    public float fadeDuration = 1f;     // lama fade in/out
    public float stayDuration = 2f;     // lama gambar diam di layar
    public string nextSceneName = "StartScene";

    void Start()
    {
        // Pastikan semua gambar transparan dulu
        foreach (Image img in images)
        {
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }

        // Mulai animasi
        StartCoroutine(PlayCutscene());
    }

    IEnumerator PlayCutscene()
    {
        for (int i = 0; i < images.Length; i++)
        {
            yield return StartCoroutine(FadeIn(images[i]));
            yield return new WaitForSeconds(stayDuration);
            yield return StartCoroutine(FadeOut(images[i]));
        }

        // Setelah semua gambar selesai, pindah scene
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeIn(Image img)
    {
        float t = 0;
        Color c = img.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            img.color = c;
            yield return null;
        }
    }

    IEnumerator FadeOut(Image img)
    {
        float t = 0;
        Color c = img.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, t / fadeDuration);
            img.color = c;
            yield return null;
        }
    }
}
    
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class CatInteraction : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioSource meowSound;
    public TextMeshProUGUI speechText;
    private bool hasInteracted = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        meowSound.Stop();
        meowSound.Play();
        Invoke(nameof(StopMeow), 2f);

        speechText.text = "Wah kamu sudah siap nih, Ayuk kita mulai!";
    }

    void StopMeow()
    {
        if (meowSound.isPlaying)
            meowSound.Stop();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!hasInteracted)
        {
            hasInteracted = true;
            SceneManager.LoadScene("ChooseCharacter"); // Pindah ke scene hasil
        }
    }
}

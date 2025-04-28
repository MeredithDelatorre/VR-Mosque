using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class UpdateTextAndFadeSound : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI panelText;
    [TextArea]
    public string newMessage = "You entered the sacred zone.";

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip loopClip;
    public float fadeDuration = 1.5f;  // How long to fade in/out

    private Coroutine fadeCoroutine;

    private void Start()
    {
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0f; // Start silent
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered sphere!");

            if (panelText != null)
                panelText.text = newMessage;

            if (audioSource != null && loopClip != null)
            {
                audioSource.clip = loopClip;
                if (!audioSource.isPlaying)
                    audioSource.Play();

                StartFade(1f); // Fade in to full volume
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited sphere.");

            if (audioSource != null)
            {
                StartFade(0f); // Fade out to silence
            }
        }
    }

    private void StartFade(float targetVolume)
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeAudio(targetVolume));
    }

    private System.Collections.IEnumerator FadeAudio(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;

        if (Mathf.Approximately(targetVolume, 0f))
        {
            audioSource.Stop(); // Fully stop after fade out
        }
    }
}

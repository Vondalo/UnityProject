using System.Collections;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public AudioSource audioSource;
    public AudioClip fadeSound;

    private void Start()
    {
        fadeCanvasGroup.alpha = 0; // Ensure the fade starts as transparent
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f)); // Fade from transparent to black
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f)); // Fade from black to transparent
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadeCanvasGroup.alpha = endAlpha;
    }

    public IEnumerator FadeOutAndIn(System.Action onFadeOutComplete)
    {
        // Play the sound at the start of the fade out
        if (fadeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fadeSound);
        }

        yield return StartCoroutine(Fade(0f, 1f)); // Fade out to black
        onFadeOutComplete.Invoke(); // Execute the teleportation or any other action
        yield return new WaitForSeconds(1f); // Wait for a moment (optional)
        yield return StartCoroutine(Fade(1f, 0f)); // Fade back in to transparent
    }
}

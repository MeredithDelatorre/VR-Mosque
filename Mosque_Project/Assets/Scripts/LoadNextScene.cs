using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SceneTransitioner : MonoBehaviour {
    [Header("Fade Settings")]
    [Tooltip("Black UI Image that covers the screen")]
    public Image fadeImage;
    [Tooltip("Seconds for each fade in/out")]
    public float fadeDuration = 1f;
    [Tooltip("Seconds to wait while fully black before loading")]
    public float delayBeforeLoad = 2f;

    [Header("What to disable while transitioning")]
    public Behaviour[] behavioursToDisable;

    bool isTransitioning = false;

    void Awake() {
        // Find scene transitioner, make sure there's only one
        var others = FindObjectsOfType<SceneTransitioner>();
        if (others.Length > 1) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // make sure fade canvas is always on top
        if (fadeImage && fadeImage.canvas != null) {
            fadeImage.canvas.overrideSorting = true;
            fadeImage.canvas.sortingOrder = 999;
        }

        // start hidden and not blocking raycasts
        fadeImage.gameObject.SetActive(false);
        fadeImage.raycastTarget = false;
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    public void OnButtonClicked(string sceneName) {
        if (isTransitioning) return;
        StartCoroutine(DoTransition(sceneName));
    }

    IEnumerator DoTransition(string sceneName) {
        isTransitioning = true;
        ToggleBehaviours(false);

        // fade OUT
        yield return StartCoroutine(FadeRoutine(0f, 1f));

        // wait full‑black
        yield return new WaitForSeconds(delayBeforeLoad);

        // load next scene
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;

        // fade‑IN starts in OnSceneLoaded
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m) {
        // fade IN
        StartCoroutine(FadeRoutine(1f, 0f));
    }

    /// <summary>
    /// Manually lerps the Image.color alpha from ‘fromAlpha’ to ‘toAlpha’ over fadeDuration.
    /// Blocks raycasts while active; when fading back in to alpha=0 we re‑enable input.
    /// </summary>
    IEnumerator FadeRoutine(float fromAlpha, float toAlpha) {
        if (fadeImage == null) {
            Debug.LogWarning("No fade image assigned in SceneTransitioner");
            yield break;
        }

        // ensure image active & blocking raycasts
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;

        // set initial alpha
        Color baseCol = fadeImage.color;
        fadeImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, fromAlpha);

        float timer = 0f;
        while (timer < fadeDuration) {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(fromAlpha, toAlpha, timer / fadeDuration);
            fadeImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, a);
            yield return null;
        }
        // ensure full fade
        fadeImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, toAlpha);

        // When loading into a new scene and fading in (toAlpha==0), hide fade image & restore input
        if (toAlpha <= 0f) {
            fadeImage.raycastTarget = false;
            fadeImage.gameObject.SetActive(false);
            ToggleBehaviours(true);
            isTransitioning = false;
        }
    }

    // Enables/disables behaviors when fading 
    void ToggleBehaviours(bool on) {
        foreach (var b in behavioursToDisable)
            if (b != null)
                b.enabled = on;
    }
}

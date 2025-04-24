using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitioner : MonoBehaviour {
    /// <summary>
    /// Singleton reference – the *newest* SceneTransitioner wins.  If the next
    /// scene already contains its own transitioner we discard the old global
    /// copy so that we never end up with two.
    /// </summary>
    private static SceneTransitioner instance;

    [Header("Fade Settings")]
    [Tooltip("Black UI Image that covers the screen")] public Image fadeImage;
    [Tooltip("Seconds for each fade in/out")] public float fadeDuration = 1f;
    [Tooltip("Seconds to wait while fully black before loading")] public float delayBeforeLoad = 2f;

    [Header("What to disable while transitioning")]
    public Behaviour[] behavioursToDisable;

    private bool isTransitioning;

    //------------------------------------------------------------------
    //  LIFECYCLE
    //------------------------------------------------------------------
    private void Awake() {
        //------------------------------------------------------------------
        // SINGLETON HAND‑OVER LOGIC
        //------------------------------------------------------------------
        if (instance == null) {
            // First ever instance – become the global copy.
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (instance != this) {
            // A previous instance exists.  Prefer the one that lives in the
            // freshly‑loaded scene (i.e. not yet in the "DontDestroyOnLoad" scene).
            if (instance.gameObject.scene.name == "DontDestroyOnLoad") {
                Destroy(instance.gameObject); // Dump the old preserved copy.
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                // The existing instance is already part of the new scene, so we
                // are the duplicate – destroy ourselves and abort further setup.
                Destroy(gameObject);
                return;
            }
        }

        // Ensure fade canvas sits on top of everything.
        if (fadeImage && fadeImage.canvas) {
            fadeImage.canvas.overrideSorting = true;
            fadeImage.canvas.sortingOrder = 999;
        }

        // Start hidden & non‑blocking.
        if (fadeImage) {
            fadeImage.gameObject.SetActive(false);
            fadeImage.raycastTarget = false;
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    //------------------------------------------------------------------
    //  PUBLIC API
    //------------------------------------------------------------------
    public void OnButtonClicked(string sceneName) {
        if (isTransitioning) return;
        StartCoroutine(DoTransition(sceneName));
    }

    //------------------------------------------------------------------
    //  TRANSITION ROUTINES
    //------------------------------------------------------------------
    private IEnumerator DoTransition(string sceneName) {
        isTransitioning = true;
        ToggleBehaviours(false);

        // Fade OUT.
        yield return StartCoroutine(FadeRoutine(0f, 1f));

        // Hold on black.
        yield return new WaitForSeconds(delayBeforeLoad);

        // Load next scene.
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        // Fade‑IN continues from OnSceneLoaded().
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        // If another SceneTransitioner is now the active singleton, let THAT
        // copy handle the fade‑in.
        if (instance != this) return;

        StartCoroutine(FadeRoutine(1f, 0f));
    }

    /// <summary>
    /// Lerps the alpha of the assigned fade image from <paramref name="fromAlpha"/>
    /// to <paramref name="toAlpha"/> over <see cref="fadeDuration"/> seconds.
    /// </summary>
    private IEnumerator FadeRoutine(float fromAlpha, float toAlpha) {
        if (!fadeImage) {
            Debug.LogWarning("SceneTransitioner: No fade image assigned");
            yield break;
        }

        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;

        Color baseCol = fadeImage.color;
        fadeImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, fromAlpha);

        float t = 0f;
        while (t < fadeDuration) {
            t += Time.deltaTime;
            float a = Mathf.Lerp(fromAlpha, toAlpha, t / fadeDuration);
            fadeImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, a);
            yield return null;
        }

        fadeImage.color = new Color(baseCol.r, baseCol.g, baseCol.b, toAlpha);

        // If we've just completed a fade‑in, restore input & deactivate image.
        if (toAlpha <= 0f) {
            fadeImage.raycastTarget = false;
            fadeImage.gameObject.SetActive(false);
            ToggleBehaviours(true);
            isTransitioning = false;
        }
    }

    //------------------------------------------------------------------
    //  HELPERS
    //------------------------------------------------------------------
    private void ToggleBehaviours(bool enable) {
        foreach (var b in behavioursToDisable)
            if (b) b.enabled = enable;
    }
}

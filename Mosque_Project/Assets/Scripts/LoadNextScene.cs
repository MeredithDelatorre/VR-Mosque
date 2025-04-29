// SceneTransitioner.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitioner : MonoBehaviour {
    // --------------------------------------------------------------
    // SINGLETON
    // --------------------------------------------------------------
    public static SceneTransitioner Instance { get; private set; }

    // --------------------------------------------------------------
    // INSPECTOR
    // --------------------------------------------------------------
    [Header("Fade Settings")]
    [Tooltip("Black UI Image that covers the screen")]
    public Image fadeImage;

    [Tooltip("Seconds for each fade in/out")] public float fadeDuration = 1f;
    [Tooltip("Seconds to wait while fully black before loading")] public float delayBeforeLoad = 2f;

    [Header("What to disable while transitioning")]
    public Behaviour[] behavioursToDisable;

    // --------------------------------------------------------------
    // STATE
    // --------------------------------------------------------------
    bool isTransitioning;
    bool fadeInNextScene = true; // set per transition request

    // --------------------------------------------------------------
    // LIFECYCLE
    // --------------------------------------------------------------
    void Awake() {
        // Singleton hand-over logic
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            if (Instance.gameObject.scene.name == "DontDestroyOnLoad") {
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
                return;
            }
        }

        // Configure fade canvas for VR if needed
        if (fadeImage && fadeImage.canvas) {
            Canvas c = fadeImage.canvas;
            if (c.renderMode == RenderMode.ScreenSpaceOverlay)
                c.renderMode = RenderMode.ScreenSpaceCamera;

            c.worldCamera = Camera.main;
            c.planeDistance = 0.25f;
            c.overrideSorting = true;
            c.sortingOrder = 999;
        }

        // Hide at start
        if (fadeImage) {
            fadeImage.gameObject.SetActive(false);
            fadeImage.raycastTarget = false;
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    // --------------------------------------------------------------
    // PUBLIC API
    // --------------------------------------------------------------
    /// <summary>
    /// Fades to black, loads <paramref name="sceneName"/>, then optionally fades back in.
    /// </summary>
    public void FadeToScene(string sceneName, bool fadeInAfterLoad = true) {
        if (isTransitioning) return;
        fadeInNextScene = fadeInAfterLoad;
        StartCoroutine(DoTransition(sceneName));
    }

    // kept for legacy button hookups
    public void OnButtonClicked(string sceneName) => FadeToScene(sceneName);

    // --------------------------------------------------------------
    // TRANSITION ROUTINES
    // --------------------------------------------------------------
    IEnumerator DoTransition(string sceneName) {
        isTransitioning = true;
        ToggleBehaviours(false);

        // Fade-out
        yield return FadeRoutine(0f, 1f);

        // Hold full-black
        yield return new WaitForSeconds(delayBeforeLoad);

        // Load next scene
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (!op.isDone) yield return null;
        // Fade-in handled in OnSceneLoaded()
    }

    void OnSceneLoaded(Scene s, LoadSceneMode m) {
        if (Instance != this) return;            // new scene has its own transitioner
        StartCoroutine(FadeInAfterLoad());
    }

    IEnumerator FadeInAfterLoad() {
        yield return null;                       // wait one frame for XR rig camera

        // update camera reference each scene
        if (fadeImage && fadeImage.canvas)
            fadeImage.canvas.worldCamera = Camera.main;

        if (fadeInNextScene) {
            // normal animated fade-in
            yield return FadeRoutine(1f, 0f);
        } else {
            // instant cut – just disable overlay & restore state
            fadeImage.raycastTarget = false;
            fadeImage.gameObject.SetActive(false);
            ToggleBehaviours(true);
            isTransitioning = false;
        }
    }

    // --------------------------------------------------------------
    // FADE UTILS
    // --------------------------------------------------------------
    IEnumerator FadeRoutine(float fromAlpha, float toAlpha) {
        if (!fadeImage) yield break;

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

        // If we've just completed a fade-in, restore state
        if (toAlpha <= 0f) {
            fadeImage.raycastTarget = false;
            fadeImage.gameObject.SetActive(false);
            ToggleBehaviours(true);
            isTransitioning = false;
        }
    }

    // --------------------------------------------------------------
    // HELPERS
    // --------------------------------------------------------------
    void ToggleBehaviours(bool enable) {
        foreach (var b in behavioursToDisable)
            if (b) b.enabled = enable;
    }
}

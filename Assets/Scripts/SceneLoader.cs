using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Screen")]
    public GameObject loadingScreen;
    public Slider loadingSlider;

    [Header("Settings")]
    public float minimumLoadTime = 6f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- RE-INTRODUCING THE EVENT SUBSCRIPTION ---
    // We subscribe to the event when this object is enabled
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // And unsubscribe when it's disabled to prevent errors
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // The public LoadScene method remains the same
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.SetActive(true);

        // --- NEW: Timer logic starts here ---
        float elapsedTime = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // The loop now continues until the REAL load is ready AND our minimum time has passed
        while (elapsedTime < minimumLoadTime || operation.progress < 0.9f)
        {
            // Increment our timer
            elapsedTime += Time.deltaTime;

            // Calculate progress based on our timer
            float timeProgress = Mathf.Clamp01(elapsedTime / minimumLoadTime);

            // Calculate progress based on the actual scene load
            float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // The progress bar will show the progress of whichever is SLOWER.
            // This ensures it fills smoothly over our minimum time for small scenes.
            float displayProgress = Mathf.Min(timeProgress, loadProgress);
            loadingSlider.value = displayProgress;

            yield return null;
        }

        // Once the loop is done, loading is complete and our minimum time is met.
        // We can now allow the scene to activate.
        operation.allowSceneActivation = true;
    }

    // --- THIS METHOD IS THE FIX ---
    // This method is called automatically by Unity AFTER the new scene is fully loaded and active.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Now that the new scene is loaded, we can safely hide the loading screen.
        loadingScreen.SetActive(false);
    }
}

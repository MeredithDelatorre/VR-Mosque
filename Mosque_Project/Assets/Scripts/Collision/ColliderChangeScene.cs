using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnCollision : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneNameToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Optional: Only trigger if the player collides
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player collided, loading scene: " + sceneNameToLoad);
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}
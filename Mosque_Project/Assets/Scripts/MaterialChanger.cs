using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public MeshRenderer targetRenderer;
    public Material newMaterial;

    private Material originalMaterial;

    void Start()
    {
        if (targetRenderer != null)
        {
            originalMaterial = targetRenderer.material;
        }
    }

    public void ChangeMaterial()
    {
        if (targetRenderer != null && newMaterial != null)
        {
            targetRenderer.material = newMaterial;
        }
    }

    public void RestoreMaterial()
    {
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }
    }
}

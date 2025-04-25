using UnityEngine;

public class FloatAndGlow : MonoBehaviour
{
    [Header("Floating")]
    public float floatAmplitude = 0.1f;
    public float floatFrequency = 1f;

    [Header("Glow Pulse")]
    public float glowMin = 0.2f;
    public float glowMax = 1.2f;
    public float glowFrequency = 1.5f;
    public Color glowColor = Color.yellow;

    private Vector3 startPos;
    private Material mat;

    void Start()
    {
        startPos = transform.position;

        Renderer renderer = GetComponent<Renderer>();
        mat = renderer.material; // create instance to avoid shared edits

        mat.EnableKeyword("_EMISSION");

        // Optional: if you're using an emission map, make sure it's set
        if (mat.GetTexture("_EmissionMap") == null)
        {
            mat.SetTexture("_EmissionMap", mat.GetTexture("_MainTex")); // reuse main texture for emission
        }

        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
    }

    void Update()
    {
        // Floating animation
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0f, floatOffset, 0f);

        // Emission intensity modulation
        float emissionStrength = Mathf.Lerp(glowMin, glowMax, (Mathf.Sin(Time.time * glowFrequency) + 1f) / 2f);
        Color finalColor = glowColor * Mathf.LinearToGammaSpace(emissionStrength);
        mat.SetColor("_EmissionColor", finalColor);
    }
}

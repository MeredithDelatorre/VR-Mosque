using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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
    private Rigidbody rb;

    void Start()
    {
        startPos = transform.position;

        Renderer renderer = GetComponent<Renderer>();
        mat = renderer.material; // create instance to avoid shared edits

        mat.EnableKeyword("_EMISSION");

        if (mat.GetTexture("_EmissionMap") == null)
        {
            mat.SetTexture("_EmissionMap", mat.GetTexture("_MainTex"));
        }

        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // Make sure Rigidbody is kinematic (no physics forces)
    }

    void FixedUpdate()
    {
        // Floating animation
        float floatOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        Vector3 nextPos = startPos + new Vector3(0f, floatOffset, 0f);
        rb.MovePosition(nextPos);
    }

    void Update()
    {
        // Emission intensity modulation
        float emissionStrength = Mathf.Lerp(glowMin, glowMax, (Mathf.Sin(Time.time * glowFrequency) + 1f) / 2f);
        Color finalColor = glowColor * Mathf.LinearToGammaSpace(emissionStrength);
        mat.SetColor("_EmissionColor", finalColor);
    }
}

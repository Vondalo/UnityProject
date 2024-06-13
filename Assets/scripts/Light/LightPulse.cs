using UnityEngine.Rendering.Universal;
using UnityEngine;

public class LightPulse : MonoBehaviour
{
    public Light2D light2D;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2.0f;
    public float pulseSpeed = 1.0f;

    private float time;

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }
        time = 0f;
    }


    void Update()
    {
        time += Time.deltaTime * pulseSpeed;
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, (Mathf.Sin(time) + 1f) / 2f);
        light2D.intensity = intensity;
    }
}
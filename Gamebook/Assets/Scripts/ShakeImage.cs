using UnityEngine;

public class ShakeImage : MonoBehaviour
{
    public float shakeDuration = 0.3f;
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;

    private Vector3 originalPos;
    private float currentShakeDuration = 0f;

    private void Start()
    {
        originalPos = transform.position;
    }

    private void Update()
    {
        if (currentShakeDuration > 0)
        {
            transform.position = originalPos + Random.insideUnitSphere * shakeAmount;

            currentShakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            currentShakeDuration = 0f;
            transform.position = originalPos;
        }
    }

    public void StartShake()
    {
        currentShakeDuration = shakeDuration;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Bark : MonoBehaviour
{
    public float Lifetime;
    public float Loudness;
    public AnimationCurve BarkCurve;
    public DoggoBehaviour barker;
    AudioSource barkSoundSource;

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
        Gizmos.DrawSphere(transform.position, GetLoudnessAt(transform.position));
    }

    // Start is called before the first frame update
    void Start()
    {
        barkSoundSource = GetComponent<AudioSource>();
        if (barkSoundSource) {
            barkSoundSource.Play();
        }
    }

    public float GetLoudnessAt(Vector3 pos) {
        return BarkCurve.Evaluate(Lifetime) * Loudness * Mathf.Max(0.0f, 1.0f - (transform.position - pos).magnitude);
    }

    // Update is called once per frame
    void Update()
    {
        Lifetime -= Time.deltaTime;
    }
}

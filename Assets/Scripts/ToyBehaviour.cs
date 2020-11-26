using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToyBehaviour : MonoBehaviour
{
    Rigidbody body;

    public void ShootUpRandomDirection() {
        Vector3 impulse = Vector3.up + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        body.AddForce(impulse, ForceMode.Impulse);
    }

    void Awake() {
        body = GetComponent<Rigidbody>();
    }
}

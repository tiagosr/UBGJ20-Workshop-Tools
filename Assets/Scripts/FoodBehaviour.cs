using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FoodBehaviour : MonoBehaviour
{

    public float FoodAmount = 1.0f;

    public float Eat(float bite) {
        float bitten = Mathf.Min(FoodAmount, bite);
        FoodAmount -= bitten;
        return bitten;
    }

    void OnDrawGizmos() {
        if (FoodAmount <= 0.0) {
            Handles.Label(transform.TransformPoint(Vector3.up), "EMPTY!", SceneManager.Instance.foodHandlesStyle);
        }
    }
}

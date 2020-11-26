using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(FoodBehaviour))]
public class FoodEditor : Editor
{
    void OnSceneGUI() {
        FoodBehaviour food = target as FoodBehaviour;
        Handles.BeginGUI();
        Vector2 location = HandleUtility.WorldToGUIPoint(food.transform.TransformPoint(Vector3.up * 4.0f));
        GUILayout.Window(0, new Rect(location, new Vector2(150, 50)), (id) => {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Replenish")) {
                Debug.Log("Replenished!");
                food.FoodAmount = 1.0f;
            }
            GUILayout.EndVertical();
        }, "Food");
        Handles.EndGUI();
    }
}

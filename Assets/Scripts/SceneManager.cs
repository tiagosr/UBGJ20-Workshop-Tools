﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GUIStyle dogHandlesStyle;
    public GUIStyle foodHandlesStyle;

    List<Bark> currentLiveBarks = new List<Bark>();


    private static SceneManager instance = null;
    public static SceneManager Instance {
        get {
            if (instance == null) {
                instance = GameObject.FindObjectOfType<SceneManager>();
            }
            return instance;
        }
    }

    public static System.WeakReference<GameObject> GetRandomToy() {
        GameObject[] toys = GameObject.FindGameObjectsWithTag("Toy");
        if (toys.Length > 0) {
            return new System.WeakReference<GameObject>(toys[Random.Range(0, toys.Length)]);
        } else {
            return null;
        }
    }

    public static System.WeakReference<GameObject> GetRandomFood() {
        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        if (food.Length > 0) {
            return new System.WeakReference<GameObject>(food[Random.Range(0, food.Length)]);
        }
        else {
            return null;
        }
    }

    public static System.WeakReference<GameObject> GetClosestNonEmptyFood(Vector3 point) {
        GameObject[] food = GameObject.FindGameObjectsWithTag("Food");
        float closestDistanceSqr = Mathf.Infinity;
        int index = Random.Range(0, food.Length);
        if (food.Length > 0) {
            for (int i = 0; i < food.Length; i++) {
                float distanceSqr = (point - food[i].transform.position).sqrMagnitude;
                FoodBehaviour foodB = food[i].GetComponent<FoodBehaviour>();
                if (foodB && distanceSqr < closestDistanceSqr && foodB.FoodAmount > 0.0f) {
                    index = i;
                    closestDistanceSqr = distanceSqr;
                }
            }
            return new System.WeakReference<GameObject>(food[index]);
        }
        return null;
    }


    public static System.WeakReference<GameObject> GetRandomDogExceptMe(GameObject me) {
        GameObject[] dogs = GameObject.FindGameObjectsWithTag("Dog");
        if (dogs.Length > 1) {
            List<GameObject> dogsMinusMe = new List<GameObject>(dogs.Length - 1);
            for(int i = 0; i < dogs.Length; i++) {
                if (dogs[i] != me) {
                    dogsMinusMe.Add(dogs[i]);
                }
            }
            return new System.WeakReference<GameObject>(dogsMinusMe[Random.Range(0, dogsMinusMe.Count)]);
        }
        else {
            return null;
        }
    }

    
    public void SpawnBark(Bark barkPrefab, Vector3 location, DoggoBehaviour barker) {
        Bark newBark = Instantiate<Bark>(barkPrefab, location, Quaternion.identity);
        newBark.barker = barker;
        currentLiveBarks.Add(newBark);
    }

    void Awake() {
        instance = this;
    }

    private void Update() {
        List<Bark> deadBarks = currentLiveBarks.FindAll((bark) => {
            return bark.Lifetime <= 0.0f;
        });
        currentLiveBarks.RemoveAll((bark) => {
            return bark.Lifetime <= 0.0f;
        });
        deadBarks.ForEach((bark) => {
            Destroy(bark.gameObject);
        });
    }
}

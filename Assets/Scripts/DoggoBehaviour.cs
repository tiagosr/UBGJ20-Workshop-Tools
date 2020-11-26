using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;


public class DoggoBehaviour : MonoBehaviour
{
    public float Hunger = 0.0f;
    public float Energy = 1.0f;
    public float HungerRate = 0.05f;
    public float HungerBite = 0.3f;
    public float EatingInterval = 1.0f;
    public float EnergySpendingPlaying = 0.03f;
    public float EnergySpendingHungry = 0.015f;
    public float ToyBoredom = 0.0f;
    public float ToyBoredomRate = 0.15f;
    public float SleepRecovery = 0.01f;
    public float NoiseTolerance = 0.2f;
    public float Annoyedness = 0.0f;
    public float SleepyTime = 0.0f;

    public Bark happyBark;
    public Bark grumpyBark;
    public Bark sadBark;

    public float BarkCooldown = 0.0f;

    NavMeshAgent agent;
    string internalState = "¯\\_(ツ)_/¯";

    System.WeakReference<GameObject> currentTarget = null;

    void OnDrawGizmos() {
        Handles.Label(transform.TransformPoint(Vector3.up), internalState, SceneManager.Instance.dogHandlesStyle);
        
    }

    Vector3 currentTargetPos;
    float lastTargetTime;

    void UpdateTarget(Vector3 targetPos, float timeGrain) {
        float currentTime = Time.timeSinceLevelLoad;
        if ((currentTime - lastTargetTime) > timeGrain && (currentTargetPos - targetPos).sqrMagnitude > 0.1f) {
            agent.SetDestination(targetPos);
            lastTargetTime = currentTime;
            currentTargetPos = targetPos;
        }
    }

    void BarkHappy() {
        if (BarkCooldown <= 0.0f) {
            SceneManager.Instance.SpawnBark(happyBark, transform.position, this);
            BarkCooldown = happyBark.Lifetime * Random.Range(1.0f, 3.0f);
        }
    }

    void BarkSad() {
        if (BarkCooldown <= 0.0f) {
            SceneManager.Instance.SpawnBark(sadBark, transform.position, this);
            BarkCooldown = sadBark.Lifetime * Random.Range(1.0f, 3.0f);
        }
    }

    void BarkGrumpy() {
        if (BarkCooldown <= 0.0f) {
            SceneManager.Instance.SpawnBark(grumpyBark, transform.position, this);
            BarkCooldown = grumpyBark.Lifetime * Random.Range(1.0f, 3.0f);
        }
    }

    System.WeakReference<GameObject> IWantThisFood = null;
    System.WeakReference<GameObject> IWantThisToy = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lastTargetTime = Time.timeSinceLevelLoad;

    }

    System.WeakReference<FoodBehaviour> ImTouchingThisFood = null;
    float FoodEatingCooldown = 0.0f;

    void EatFood(FoodBehaviour food) {
        if (FoodEatingCooldown <= 0.0f && food.FoodAmount >= 0.0f) {
            Hunger -= food.Eat(HungerBite);
            FoodEatingCooldown += EatingInterval;
        }
    }

    void OnTriggerEnter(Collider other) {
        FoodBehaviour food = other.GetComponent<FoodBehaviour>();
        if (food) {
            ImTouchingThisFood = new System.WeakReference<FoodBehaviour>(food);
        }
    }

    void OnTriggerExit(Collider other) {
        FoodBehaviour food = other.GetComponent<FoodBehaviour>();
        FoodBehaviour imTouchingThisFood;
        if (ImTouchingThisFood != null && ImTouchingThisFood.TryGetTarget(out imTouchingThisFood) && food == imTouchingThisFood) {
            ImTouchingThisFood = null;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Toy")) {
            // throw toy upwards at a random direction
            ToyBehaviour toy = collision.collider.GetComponent<ToyBehaviour>();
            if (toy) {
                toy.ShootUpRandomDirection();
            }
        }
    }



    void Update()
    {
        FoodEatingCooldown = Mathf.Max(0.0f, FoodEatingCooldown - Time.deltaTime);
        BarkCooldown = Mathf.Max(0.0f, BarkCooldown - Time.deltaTime);
        if (SleepyTime > 0.0f) {
            float noiseLevel = 0.0f;
            if (noiseLevel > 0.2f) {
                // wake up grumpy and go somewhere else quieter つ´Д`)つ
                internalState = "つ´Д`)つ";
                BarkGrumpy();

            }
            else {
                // sleepy time ( ु⁎ᴗ_ᴗ⁎)ु.｡zZ
                internalState = "( ु⁎ᴗ_ᴗ⁎)ु.｡zZ";
                agent.isStopped = true;
                Energy += Time.deltaTime * SleepRecovery;
                SleepyTime -= Time.deltaTime;
            }
        } else if (Energy < 0.1f) {
            SleepyTime = Mathf.Min(1.0f, SleepyTime + 1.0f);
        } else if (Hunger > 0.5f) {
            GameObject iWantThisFood = null;
            if (IWantThisFood != null && IWantThisFood.TryGetTarget(out iWantThisFood)) {
                // walk towards food
                FoodBehaviour imTouchingThisFood = null;
                if (ImTouchingThisFood != null && ImTouchingThisFood.TryGetTarget(out imTouchingThisFood)) {
                    // yum yum ლ(´ڡ`ლ)
                    internalState = "ლ(´ڡ`ლ)";
                    EatFood(imTouchingThisFood);
                }
                else {
                    // go walk towards food ԅ(♡﹃♡ԅ)
                    internalState = "ԅ(♡﹃♡ԅ)";
                    UpdateTarget(iWantThisFood.transform.position, 1.0f);
                }
            }
            else {
                System.WeakReference<GameObject> foodObject = SceneManager.GetRandomFood();
                if (foodObject.TryGetTarget(out iWantThisFood)) {
                    IWantThisFood = foodObject;
                }
                else {
                    // whaaaa? no food around Щ(º̩̩́Дº̩̩̀щ)
                    internalState = "Щ(º̩̩́Дº̩̩̀щ)";
                    // sulk and bark a bit
                    BarkSad();
                }
            }
        }
        else {
            // go play!
            internalState = "(＾ω＾)";
            Energy -= Time.deltaTime * EnergySpendingPlaying;
            Hunger += Time.deltaTime * HungerRate;
            ToyBoredom += Time.deltaTime * ToyBoredomRate;
            if (ToyBoredom >= 1.0f) {
                IWantThisToy = null;
                ToyBoredom = 0.0f;
            }
            GameObject iWantThisToy = null;
            if (IWantThisToy != null && IWantThisToy.TryGetTarget(out iWantThisToy)) {
                UpdateTarget(iWantThisToy.transform.position, 2.0f);
                BarkHappy();
            }
            else {
                System.WeakReference<GameObject> toyObject = SceneManager.GetRandomToy();
                if (toyObject.TryGetTarget(out iWantThisToy)) {
                    IWantThisToy = toyObject;
                }
                else {

                }
            }
        }
    }
}

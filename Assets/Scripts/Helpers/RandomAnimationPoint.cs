using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Obsolete]
public class RandomAnimationPoint : MonoBehaviour
{
    [SerializeField] private bool randomize;
    [Range(0f, 1f), SerializeField] private float normalizedTime;
    
    void OnValidate()
    {
        GetComponent<Animator>().Update(0f);
        GetComponent<Animator>().Play("Walk", 0, randomize ? Random.value : normalizedTime);
    }
}
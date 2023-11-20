using UnityEngine;

public class RandomParticlePoint : MonoBehaviour 
{
    [Range(0f, 1f), SerializeField]
    private float normalizedTime;
    
    void OnValidate()
    {
        GetComponent<ParticleSystem>().Simulate(normalizedTime, true, true);
    }
}

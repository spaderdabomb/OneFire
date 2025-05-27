using UnityEngine;

public class ParticleStopHandler : MonoBehaviour
{
    [SerializeField] private GameObject objToDestroy;
    public void OnParticleSystemStopped()
    {
        Destroy(objToDestroy);
    }
}
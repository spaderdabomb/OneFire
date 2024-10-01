using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSplash : MonoBehaviour
{
    private ParticleSystem splashParticleSystem;
    private float playDuration = 0f;
    public void OnInstantiate(FishData fishData, GameObject currentBob)
    {
        transform.position = currentBob.transform.position;
        ParticleSystemRestarted();
    }

    private void OnEnable()
    {
        FishingManager.Instance.OnFishHooked += DestroyParticleSystem;
        FishingManager.Instance.OnStopFishing += DestroyParticleSystem;
    }

    private void OnDisable()
    {
        FishingManager.Instance.OnFishHooked -= DestroyParticleSystem;
        FishingManager.Instance.OnStopFishing -= DestroyParticleSystem;
    }

    private void Start()
    {
        splashParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        playDuration += Time.deltaTime;
        if (playDuration > splashParticleSystem.main.duration)
        {
            ParticleSystemRestarted();
        }
    }

    private void ParticleSystemRestarted()
    {
        playDuration = 0f;
        AudioManager.PlaySound(MainLibrarySounds.FishSplashSmall);
    }

    private void DestroyParticleSystem(FishData fishData)
    {
        Destroy(gameObject);
    }

    private void DestroyParticleSystem()
    {
        Destroy(gameObject);
    }
}

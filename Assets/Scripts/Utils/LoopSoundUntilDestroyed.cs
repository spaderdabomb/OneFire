using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

public class LoopSoundUntilDestroyed : MonoBehaviour
{
    [SerializeField] MainLibrarySounds soundEffect;

    private void Awake()
    {
        AudioManager.PlaySound(soundEffect);
    }

    private void OnDestroy()
    {
        AudioManager.StopSound(soundEffect);
    }
}

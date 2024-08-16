using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LumberjackingManager : MonoBehaviour
{
    public static LumberjackingManager Instance;

    public GameObject pineTree01;

    public ParticleSystem hitSmokeEffect;
    public ParticleSystem hitSpintersEffect;
    public ParticleSystem hitFlashEffect;
    public ParticleSystem destroyEffect;
    public ParticleSystem fallingDebrisEffect;
}

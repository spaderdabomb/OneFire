using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleResourceManager : MonoBehaviour
{
    public static DestructibleResourceManager Instance;

    [Header("General Effects")]
    public ParticleSystem destroyFlashEffect;

    [Header("Tree Effects")]
    public ParticleSystem treeHitSmokeEffect;
    public ParticleSystem treeHitSplintersEffect;
    public ParticleSystem treeHitFlashEffect;
    public ParticleSystem treeDestroyEffect;
    public ParticleSystem treeFallingDebrisEffect;

    [Header("Mining Effects")]
    public ParticleSystem miningHitSmokeEffect;
    public ParticleSystem miningeHitSplintersEffect;
    public ParticleSystem miningHitFlashEffect;
    public ParticleSystem miningDestroyEffect;

    [Header("Audio")]
    [SerializeField] private SoundFileObject[] _miningSounds;
    [SerializeField] private SoundFileObject[] _lumberjackingSounds;

    private int _lastMiningSoundIndex = -1;
    private int _lastLumberjackingSoundIndex = -1;


    private void Awake()
    {
        Instance = this;
    }

    public Vector3 GetHitPosition(GameObject obj)
    {
        Vector3 interactPosition = GameObjectManager.Instance.playerInteract.transform.position;
        Vector3 objPosition = obj.transform.position;
        objPosition.y = interactPosition.y;

        float radius = obj.GetComponent<CapsuleCollider>().radius;
        Vector3 direction = (interactPosition - objPosition).normalized * radius;
        objPosition = new Vector3(objPosition.x + direction.x, objPosition.y, objPosition.z + direction.z);

        return objPosition;
    }

    public void SpawnHitEffect(ParticleSystem particleSystem, Vector3 position)
    {
        ParticleSystem hitEffect = Instantiate(particleSystem, GameObjectManager.Instance.effectsContainer.transform);
        hitEffect.transform.position = position;
    }

    public void StartTreeRespawn(WorldTree worldTree, float respawnTime)
    {
        StartCoroutine(RespawnTree(worldTree, respawnTime));
    }

    public void StartVeinRespawn(WorldVein worldVein, float respawnTime)
    {
        StartCoroutine(RespawnVein(worldVein, respawnTime));
    }

    private IEnumerator RespawnTree(WorldTree worldTree, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        worldTree.ReactivateObject();
    }

    private IEnumerator RespawnVein(WorldVein worldVein, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        worldVein.ReactivateObject();
    }

    public SoundFileObject SelectRandomLumberjackingSound(float healthPercent)
    {
        (SoundFileObject sfo, int index) = SelectRandomSound(_lumberjackingSounds, _lastLumberjackingSoundIndex, healthPercent);
        _lastLumberjackingSoundIndex = index;

        return sfo;
    }

    public SoundFileObject SelectRandomMiningSound(float healthPercent)
    {
        (SoundFileObject sfo, int index) = SelectRandomSound(_miningSounds, _lastMiningSoundIndex, healthPercent);
        _lastMiningSoundIndex = index;

        return sfo;
    }

    private (SoundFileObject, int) SelectRandomSound(SoundFileObject[] sounds, int lastIndex, float healthPercent)
    {
        if (healthPercent == 1f)
            return (sounds[0], 0);

        int newIndex;
        do
        {
            newIndex = Random.Range(0, sounds.Length);
        } while (newIndex == lastIndex);

        lastIndex = newIndex;
        return (sounds[newIndex], newIndex);
    }
}

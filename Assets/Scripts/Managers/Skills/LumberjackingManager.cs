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
    public ParticleSystem destroyFlashEffect;
    public ParticleSystem fallingDebrisEffect;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnHitEffect(GameObject obj, ParticleSystem particleSystem)
    {
        Vector3 interactPosition = GameObjectManager.Instance.playerInteract.transform.position;
        Vector3 objPosition = obj.transform.position;
        objPosition.y = interactPosition.y;

        float radius = obj.GetComponent<CapsuleCollider>().radius;
        Vector3 direction = (interactPosition - objPosition).normalized * radius;
        objPosition = new Vector3(objPosition.x + direction.x, objPosition.y, objPosition.z + direction.z);
        ParticleSystem hitEffect = Instantiate(particleSystem, GameObjectManager.Instance.effectsContainer.transform);
        hitEffect.transform.position = objPosition;
    }

    public void StartTreeRespawn(WorldTree worldTree, float respawnTime)
    {
        StartCoroutine(RespawnTree(worldTree, respawnTime));
    }

    private IEnumerator RespawnTree(WorldTree worldTree, float respawnTime)
    {
        yield return new WaitForSeconds(respawnTime);

        worldTree.ReactivateObject();
    }
}

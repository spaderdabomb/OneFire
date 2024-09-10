using GinjaGaming.FinalCharacterController;
using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(InteractingObject))]
public class WorldVein : DamageableObject, IRespawnable
{
    public VeinData veinData;
    private float _damageRemainder;

    public float RespawnTime => veinData.respawnTime;

    public override void Damage()
    {
        base.Damage();

        float damage = GameObjectManager.Instance.playerStats.CalculateDamage(gameObject);
        ItemData itemDataCloned = veinData.itemCollected.CloneItemData();
        int newStackCount = (int)Mathf.Floor(veinData.orePerHealth * (damage + _damageRemainder));
        itemDataCloned.stackCount = newStackCount;
        _damageRemainder = (_damageRemainder + damage) - newStackCount / veinData.orePerHealth;
        GameObjectManager.Instance.SpawnItem(itemDataCloned);

        Vector3 hitPosition = DestructibleResourceManager.Instance.GetHitPosition(gameObject);
        DestructibleResourceManager.Instance.SpawnHitEffect(DestructibleResourceManager.Instance.miningHitFlashEffect, hitPosition);
        DestructibleResourceManager.Instance.SpawnHitEffect(DestructibleResourceManager.Instance.miningHitSmokeEffect, hitPosition);
        DestructibleResourceManager.Instance.SpawnHitEffect(DestructibleResourceManager.Instance.miningeHitSplintersEffect, hitPosition);
        UiManager.Instance.uiGameManager.SpawnDamageNumberMesh(UiManager.Instance.damageNumberStandard, damage, hitPosition);

        SoundFileObject sfo = DestructibleResourceManager.Instance.SelectRandomMiningSound();
        AudioManager.PlaySound(sfo);
    }

    public override void DeactivateObject()
    {
        Vector3 hitPosition = DestructibleResourceManager.Instance.GetHitPosition(gameObject);
        DestructibleResourceManager.Instance.StartVeinRespawn(this, RespawnTime);
        DestructibleResourceManager.Instance.SpawnHitEffect(DestructibleResourceManager.Instance.miningDestroyEffect, hitPosition);
        DestructibleResourceManager.Instance.SpawnHitEffect(DestructibleResourceManager.Instance.destroyFlashEffect, hitPosition);
        AudioManager.PlaySound(MainLibrarySounds.TreeDestroy_02);

        base.DeactivateObject();

        if (transform.parent.GetComponent<LODGroup>() != null)
            transform.parent.gameObject.SetActive(false);

        ItemData itemDataCloned = veinData.itemCollected.CloneItemData();
        itemDataCloned.stackCount = veinData.oreWhenDestroyed;
        GameObjectManager.Instance.SpawnItem(itemDataCloned);
    }

    public void ReactivateObject()
    {
        gameObject.SetActive(true);
        if (transform.parent.GetComponent<LODGroup>() != null)
            transform.parent.gameObject.SetActive(true);
    }

    public override void DestroyObject()
    {
        base.DestroyObject();

        ItemData itemDataCloned = veinData.itemCollected.CloneItemData();
        itemDataCloned.stackCount = veinData.oreWhenDestroyed;
        GameObjectManager.Instance.SpawnItem(itemDataCloned);
    }


    private void OnValidate()
    {
#if UNITY_EDITOR
        if (transform.parent.GetComponent<LODGroup>() != null)
        {
            Transform parent = transform.parent;
            if (PrefabUtility.IsPartOfPrefabAsset(parent))
            {
                GameObject gameObj = PrefabUtility.GetCorrespondingObjectFromOriginalSource(veinData.veinPrefab);
                if (gameObj.name != parent.gameObject.name)
                {
                    Debug.LogError($"{this} Prefab not set to correct data ");
                }
            }
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.collisionDetectionMode == CollisionDetectionMode.Discrete)
        {
            Debug.Log($"{this} collision detection set to discrete - switching to continuous");
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        if (!rb.isKinematic)
        {
            Debug.Log($"{this} isKinematic not set - switching to isKinematic");
            rb.isKinematic = true;
        }
#endif
    }
}

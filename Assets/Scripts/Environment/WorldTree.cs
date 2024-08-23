using GinjaGaming.FinalCharacterController;
using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CapsuleCollider), typeof(Rigidbody), typeof(InteractingObject))]
public class WorldTree : DamageableObject, IRespawnable
{
    public TreeData treeData;
    private float _damageRemainder;

    public float RespawnTime => treeData.respawnTime;

    public override void Damage()
    {
        base.Damage();

        float damage = GameObjectManager.Instance.playerStats.CalculateDamage(gameObject);
        ItemData itemDataCloned = treeData.itemCollected.CloneItemData();
        int newStackCount = (int)Mathf.Floor(treeData.logsPerHealth * (damage + _damageRemainder));
        itemDataCloned.stackCount = newStackCount;
        _damageRemainder = (_damageRemainder + damage) - newStackCount / treeData.logsPerHealth;
        GameObjectManager.Instance.SpawnItem(itemDataCloned);

        Vector3 hitPosition = LumberjackingManager.Instance.GetHitPosition(gameObject);
        LumberjackingManager.Instance.SpawnHitEffect(LumberjackingManager.Instance.hitFlashEffect, hitPosition);
        LumberjackingManager.Instance.SpawnHitEffect(LumberjackingManager.Instance.hitSmokeEffect, hitPosition);
        LumberjackingManager.Instance.SpawnHitEffect(LumberjackingManager.Instance.hitSpintersEffect, hitPosition);
        UiManager.Instance.uiGameManager.SpawnDamageNumber(UiManager.Instance.damageNumberStandard, damage, hitPosition);
        AudioManager.PlaySound(MainLibrarySounds.FistHitTree_02);
    }

    public override void DeactivateObject()
    {
        Vector3 hitPosition = LumberjackingManager.Instance.GetHitPosition(gameObject);
        LumberjackingManager.Instance.StartTreeRespawn(this, RespawnTime);
        LumberjackingManager.Instance.SpawnHitEffect(LumberjackingManager.Instance.destroyEffect, hitPosition);
        LumberjackingManager.Instance.SpawnHitEffect(LumberjackingManager.Instance.destroyFlashEffect, hitPosition);
        AudioManager.PlaySound(MainLibrarySounds.TreeDestroy_02);

        base.DeactivateObject();

        if (transform.parent.GetComponent<LODGroup>() != null)
            transform.parent.gameObject.SetActive(false);

        ItemData itemDataCloned = treeData.itemCollected.CloneItemData();
        itemDataCloned.stackCount = treeData.logsWhenDestroyed;
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

        ItemData itemDataCloned = treeData.itemCollected.CloneItemData();
        itemDataCloned.stackCount = treeData.logsWhenDestroyed;
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
                GameObject gameObj = PrefabUtility.GetCorrespondingObjectFromOriginalSource(treeData.treePrefab);
                if (gameObj.name != parent.gameObject.name)
                {
                    Debug.LogError($"{this} Tree prefab not set to correct treeData");
                }
            }
        }

        if (GetComponent<Rigidbody>().collisionDetectionMode == CollisionDetectionMode.Discrete)
        {
            Debug.Log($"{this} collision detection set to discrete - switching to continuous");
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
#endif
    }
}

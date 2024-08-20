using GinjaGaming.FinalCharacterController;
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

        LumberjackingManager.Instance.SpawnHitEffect(gameObject, LumberjackingManager.Instance.hitFlashEffect);
        LumberjackingManager.Instance.SpawnHitEffect(gameObject, LumberjackingManager.Instance.hitSmokeEffect);
    }

    public override void DeactivateObject()
    {
        LumberjackingManager.Instance.StartTreeRespawn(this, RespawnTime);
        LumberjackingManager.Instance.SpawnHitEffect(gameObject, LumberjackingManager.Instance.destroyEffect);
        LumberjackingManager.Instance.SpawnHitEffect(gameObject, LumberjackingManager.Instance.destroyFlashEffect);

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
        if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
        {
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                GameObject gameObj = PrefabUtility.GetCorrespondingObjectFromOriginalSource(treeData.treePrefab);
                if (gameObj == gameObject)
                {
                    Debug.LogError($"{this} Tree prefab not set to correct treeData");
                }
            }
        }
#endif
    }
}

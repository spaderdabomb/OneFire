using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "OneFire/Items/FishData")]
public class FishData : ItemData
{
    [Header("Fish Parameters")]
    public float catchExp = 10f;
    public float escapeBonus = 1f;
    public float timeToEscape = 10f;
    public int catchSegments = 3;

    public override void OnValidate()
    {
        base.OnValidate();

        FishRegistry.Register(this);
    }
}

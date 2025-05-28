using UnityEngine.UIElements;

public interface ITabInterface
{
    public void RegisterCallbacks();
    public void UnregisterCallbacks();
    public void TabIndexChanged(ChangeEvent<bool> value);
    public void SetTabSelectedValue(bool value);
    public void OnHover(PointerEnterEvent evt);
}

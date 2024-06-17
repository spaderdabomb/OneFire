using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IUiToolkitElement
{
    void RegisterCallbacks();
    void UnregisterCallbacks();
    void OnGeometryChanged(GeometryChangedEvent evt);
}

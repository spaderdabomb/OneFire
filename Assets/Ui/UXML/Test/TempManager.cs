using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TempManager : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;

    [SerializeField] private VisualTreeAsset baseTemplateAsset;
    [SerializeField] private VisualTreeAsset derivedTemplateAsset;
    BaseTemplate baseTemplate;
    DerivedTemplate derivedTemplate;
    void Start()
    {
        VisualElement baseClone = baseTemplateAsset.CloneTree();
        //VisualElement derivedClone = derivedTemplateAsset.CloneTree();
        derivedTemplate = new DerivedTemplate(uiDocument.rootVisualElement);
    }
}

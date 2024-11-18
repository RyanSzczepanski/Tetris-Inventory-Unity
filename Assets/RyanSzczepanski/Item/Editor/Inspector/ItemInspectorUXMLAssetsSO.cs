using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Items/Editor/EditorAssetTrees", fileName = "New Editor Asset Trees")]
public class ItemInspectorUXMLAssetsSO : ScriptableObject
{
    public VisualTreeAsset ItemBaseSO => m_ItemBaseSO;
    public VisualTreeAsset ItemIInventoryInterface => m_ItemIInventoryInterface;

    [SerializeField] VisualTreeAsset m_ItemBaseSO;
    [SerializeField] VisualTreeAsset m_ItemIInventoryInterface;
}

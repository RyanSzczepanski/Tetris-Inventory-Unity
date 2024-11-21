using UnityEngine;

[System.Serializable]
public abstract class ItemBaseSO : ScriptableObject
{
    [SerializeField] private string m_FullName;
    [SerializeField] private string m_ShortName;
    [SerializeField] private string m_Description;
    [SerializeField] private int m_Value;
    [SerializeField] private float m_Weight;
    [SerializeField] private Vector2Int m_Size;
    [SerializeField] private Sprite m_Icon;

    public string FullName => m_FullName;
    public string ShortName => m_ShortName;
    public string Description => m_Description;
    public int Value => m_Value;
    public float Weight => m_Weight;
    public Vector2Int Size => m_Size;
    public Sprite Icon => m_Icon;
    public abstract ItemTags Tags { get; }

    public virtual ItemBase CreateItem()
    {
        return new ItemBase(this);
    }
}

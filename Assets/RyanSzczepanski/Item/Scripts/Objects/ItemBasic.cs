///------------------------------------///
///    This File Was Auto Generated    ///
/// Using Szczepanski Script Generator ///
///------------------------------------///

using Szczepanski.UI;

public class ItemBasic : ItemBase
{
    #region Properties
    #region Base
    public new ItemBasicSO Data { get; }
    public override ContextMenuOption[] ContextMenuOptions
    {
        get => new ContextMenuOption[]
        {
            new ContextMenuOption { optionText = "Inspect", OnSelected = () => throw new System.NotImplementedException("Inspect Item Not Implemented")},
            new ContextMenuOption { optionText = "Discard", OnSelected = () => ParentSubInventory.TryRemoveItem(this)}
        };
    }
    #endregion
    #endregion
    #region Functions
    #region Base
    public ItemBasic(ItemBasicSO data) : base(data)
    {
        Data = data;
    }
    #endregion
    #endregion
}
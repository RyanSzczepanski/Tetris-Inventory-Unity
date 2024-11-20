using Szczepanski.UI;

public class ItemInventory : ItemBase, IInventory
{
    public new ItemInventorySO Data { get; private set; }
    public Inventory Inventory { get; private set; }

    public ItemInventory(ItemInventorySO data) : base(data)
    {
        Data = data;
        Inventory = new Inventory(data);
    }

    //TODO: Make this more modular using the interfaces
    public override ContextMenuOption[] ContextMenuOptions
    {
        get => new ContextMenuOption[3]
        {
            new ContextMenuOption { optionText = "Inspect", OnSelected = () => throw new System.NotImplementedException("Inspect Item Not Implemented")},
            new ContextMenuOption { optionText = "Open", OnSelected = () => IInventory.OpenUI(this, InventoryUIManager.CANVAS.transform)},
            new ContextMenuOption { optionText = "Discard", OnSelected = () => ParentSubInventory.TryRemoveItem(this)},
        };
    }

    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}\n   Storage: {Data.StorageSlots}";
    }
}

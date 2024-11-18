public class ItemInventory : ItemBase, IInventory
{
    public new ItemInventorySO Data { get; private set; }
    public Inventory Inventory { get; private set; }

    public ItemInventory(ItemInventorySO data) : base(data)
    {
        Data = data;
        Inventory = new Inventory(data);
    }

    public override string ToString()
    {
        return $"{Data.FullName}\n   Type: {GetType()}\n   Size: {Data.Size}\n   Storage: {Data.StorageSlots}";
    }
}

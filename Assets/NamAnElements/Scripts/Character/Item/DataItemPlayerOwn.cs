[System.Serializable]
public class DataItemPlayerOwn
{
    public string itemName;
    public int amount;

    public DataItemPlayerOwn(string itemName, int amount)
    {
        this.itemName = itemName;
        this.amount = amount;
    }
}

public interface IStorageProvider
{
    public BurningEntry GetLatestEntry();
    public void SaveEntry(BurningEntry entry);
    public IEnumerable<BurningEntry> GetAllEntries();
}
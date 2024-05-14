public class StorageProvider : IStorageProvider
{
    private readonly string _filename;

    public StorageProvider()
    {
        _filename = "Burned List.txt";
    }
    public BurningEntry GetLatestEntry()
    {
        var lastLine = File.ReadAllLines(_filename).Last();
        var response = new BurningEntry()
        {
            BurningTime = DateTime.Parse(lastLine)
        };
        return response;
    }

    public void SaveEntry(BurningEntry entry)
    {
        var linesToAppend = new List<string>() { entry.BurningTime.ToString() };
        File.AppendAllLines(_filename, linesToAppend);
    }

    public IEnumerable<BurningEntry> GetAllEntries()
    {
        var lines = File.ReadAllLines(_filename);
        var entries = lines.Select(x => new BurningEntry() { BurningTime = Convert.ToDateTime(x) }).ToList();
        return entries;
    }
}
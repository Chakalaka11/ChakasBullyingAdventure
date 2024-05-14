public class ChakaAssBurnedManager
{
    private DateTime _assBurnedTime = Convert.ToDateTime("2020/01/01T00:00:00");
    private static TimeSpan _burningTimespan;
    private static TimeSpan _goodTimesTimespan;
    private static IStorageProvider _storageProvider;

    public ChakaAssBurnedManager()
    {
        _burningTimespan = TimeSpan.FromMinutes(1);
        _goodTimesTimespan = TimeSpan.FromDays(1);
        _storageProvider = new StorageProvider();
    }

    public void AssBurned(DateTime timeWhenStarted)
    {
        _assBurnedTime = timeWhenStarted;
        _storageProvider.SaveEntry(new BurningEntry() { BurningTime = _assBurnedTime });
    }

    public bool IsChakaBurning()
    {
        if (DateTime.Now - _assBurnedTime <= _burningTimespan)
            return true;
        else
            return false;
    }

    public bool IsChakaChill()
    {
        if (DateTime.Now - _assBurnedTime >= _goodTimesTimespan)
            return true;
        else
            return false;
    }

    public string GetBurningMessage()
    {
        if (IsChakaBurning())
            return "HE IS BURNING RIGHT NOW";
        else
        {
            if (IsChakaChill())
                return "Chaka is pretty chill";
            else
            {
                var time = DateTime.Now - _assBurnedTime;
                var msg = "Chaka burned ";
                if (time.Hours > 0)
                    msg += $"{time.Hours} hours and ";
                msg += $"{time.Minutes} minutes ago";
                return msg;
            }
        }
    }
}

public class GroupedDataByDate
{
    public IEnumerable<CountOfDataInTime> Hashes { get; set; }

    public GroupedDataByDate(IEnumerable<CountOfDataInTime> hashes)
    {
        Hashes = hashes;
    }
}

public class CountOfDataInTime
{
    public string Date { get; set; }
    public int Count { get; set; }

    public CountOfDataInTime(string date, int count)
    {
        Date = date;
        Count = count;
    }
}
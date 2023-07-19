namespace DataGeneratorApi.Services.Abstractions;

public interface IHashService
{
    Task<int> GenerateAndSendHashesAsync(int numberOfHashes);
    Task<GroupedDataByDate> GetGroupedDataByDates();
}
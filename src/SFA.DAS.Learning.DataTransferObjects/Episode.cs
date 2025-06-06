using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Apprenticeships.DataTransferObjects;

[ExcludeFromCodeCoverage]
public class Episode
{
    public Episode(Guid key, string trainingCode, List<EpisodePrice> prices)
    {
        Key = key;
        TrainingCode = trainingCode;
        Prices = prices;
    }

    public Guid Key { get; set; }
    public string TrainingCode { get; set; }
    public List<EpisodePrice> Prices { get; set; }
}
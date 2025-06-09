namespace SFA.DAS.Learning.Queries.GetLearningsWithEpisodes;

public class GetLearningsWithEpisodesRequest : IQuery
{
    public long Ukprn { get; set; }
    public short CollectionYear { get; set; }
    public byte CollectionPeriod { get; set; }
}
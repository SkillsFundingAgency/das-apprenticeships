namespace SFA.DAS.Learning.Queries.GetApprenticeshipsWithEpisodes;

public class GetApprenticeshipsWithEpisodesRequest : IQuery
{
    public long Ukprn { get; set; }
    public short CollectionYear { get; set; }
    public byte CollectionPeriod { get; set; }
}
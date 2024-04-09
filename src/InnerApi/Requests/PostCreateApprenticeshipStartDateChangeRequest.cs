namespace SFA.DAS.Apprenticeships.InnerApi.Requests;

public class PostCreateApprenticeshipStartDateChangeRequest
{
    public string Initiator { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime ActualStartDate { get; set; }
    public string Reason { get; set; } = null!;
}
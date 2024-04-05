namespace SFA.DAS.Apprenticeships.InnerApi.Requests;

public class PostCreateApprenticeshipStartDateChangeRequest
{
    public string Initiator { get; set; }
    public string UserId { get; set; }
    public DateTime ActualStartDate { get; set; }
    public string Reason { get; set; }
}
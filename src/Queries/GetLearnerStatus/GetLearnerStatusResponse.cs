using System.ComponentModel;

namespace SFA.DAS.Apprenticeships.Queries.GetLearnerStatus;

public class GetLearnerStatusResponse
{
    public LearnerStatus LearnerStatus { get; set; }
}

//PR DISCUSSION POINT: should we put this somewhere else?
public enum LearnerStatus
{
    None,

    [Description("Waiting to start")]
    WaitingToStart,

    [Description("In learning")]
    InLearning,

    [Description("Break in learning")]
    BreakInLearning,

    [Description("Withdrawn")]
    Withdrawn,

    [Description("Completed")]
    Completed
}
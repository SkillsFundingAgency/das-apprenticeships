﻿using System.ComponentModel;

namespace SFA.DAS.Apprenticeships.Types;

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

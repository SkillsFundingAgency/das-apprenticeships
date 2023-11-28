﻿namespace SFA.DAS.Apprenticeships.Domain.Repositories
{
    public interface IApprenticeshipRepository
    {
        Task Add(Apprenticeship.Apprenticeship apprenticeship);
        Task<Apprenticeship.Apprenticeship> Get(Guid key);
        Task Update(Apprenticeship.Apprenticeship apprenticeship);
    }
}

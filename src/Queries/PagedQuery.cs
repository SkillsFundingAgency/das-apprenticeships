﻿namespace SFA.DAS.Learning.Queries;

public abstract class PagedQuery
{
    public int Page { get; set; }
    public int? PageSize { get; set; }

    public int Limit => PageSize ?? int.MaxValue;
    public int Offset => PageSize.HasValue ? (Page - 1) * PageSize.Value : 0;
}

public abstract class PagedQueryResult<T>
{
    public int Page { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public IEnumerable<T> Items { get; set; }
}
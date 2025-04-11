﻿namespace Persistence.SeedWorks.Implements;

public class BaseTrackableEntity<T> : BaseAuditableEntity<T> where T : class
{
    public required T CreatedBy { get; set; }
    public T? UpdatedBy { get; set; }
}

using System;
using System.Collections.Generic;

namespace Persistence.Entity;

public partial class Transaction
{
    public int Id { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public long Price { get; set; }

    public string Status { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string? CodeTransaction { get; set; }

    public Guid? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}

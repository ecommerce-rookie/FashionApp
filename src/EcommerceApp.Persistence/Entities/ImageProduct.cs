using System;
using System.Collections.Generic;

namespace Persistence.Entity;

public partial class ImageProduct
{
    public string Image { get; set; } = null!;

    public Guid ProductId { get; set; }

    public int? OrderNumber { get; set; }

    public virtual Product Product { get; set; } = null!;
}

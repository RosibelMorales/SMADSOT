using System;
using System.Collections.Generic;

namespace Smadot.Models.DataBase;

public partial class Error
{
    public long Id { get; set; }

    public string Values { get; set; } = null!;

    public DateTime Created { get; set; }
}

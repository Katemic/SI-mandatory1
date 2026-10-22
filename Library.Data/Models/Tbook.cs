using System;
using System.Collections.Generic;

namespace Library.Data.Models;

public partial class Tbook
{
    public int NBookId { get; set; }

    public string CTitle { get; set; } = null!;

    public int NAuthorId { get; set; }

    public int NPublishingYear { get; set; }

    public int NPublishingCompanyId { get; set; }

    public virtual Tauthor NAuthor { get; set; } = null!;

    public virtual Tpublishingcompany NPublishingCompany { get; set; } = null!;
}

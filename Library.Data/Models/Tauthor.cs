using System;
using System.Collections.Generic;

namespace Library.Data.Models;

public partial class Tauthor
{
    public int NAuthorId { get; set; }

    public string CName { get; set; } = null!;

    public string CSurname { get; set; } = null!;

    public virtual ICollection<Tbook> Tbooks { get; set; } = new List<Tbook>();
}

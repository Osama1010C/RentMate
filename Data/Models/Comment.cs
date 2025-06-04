using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int PropertyId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public virtual Property Property { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

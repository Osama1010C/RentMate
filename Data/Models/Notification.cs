using System;
using System.Collections.Generic;

namespace RentMateAPI.Data.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Description { get; set; } = null!;

    public string NotificationType { get; set; } = null!;

    public int NotificationTypeId { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual User User { get; set; } = null!;
}

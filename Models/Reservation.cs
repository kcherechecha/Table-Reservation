using System;
using System.Collections.Generic;

namespace WebRestik.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public DateTime BookingTime { get; set; }

    public int TableId { get; set; }

    public string ClientName { get; set; }

    public int WaiterId { get; set; }

    public virtual Table Table { get; set; }

    public virtual Waiter Waiter { get; set; }
}

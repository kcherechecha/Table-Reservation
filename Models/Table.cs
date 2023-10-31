using System;
using System.Collections.Generic;

namespace WebRestik.Models;

public partial class Table
{
    public int Id { get; set; }

    public string Number { get; set; }

    public int RestaurantId { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Restaurant Restaurant { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebRestik.Models;

public partial class Waiter
{
    public int Id { get; set; }

    public string Name { get; set; }
    [Display(Name = "Restaurant")]
    public int RestaurantId { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual Restaurant Restaurant { get; set; }
}

using System;
using System.Collections.Generic;

namespace WebRestik.Models;

public partial class Restaurant
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Location { get; set; }

    public double Latitude { get; set; }

    public double Longtitude { get; set; }

    public virtual ICollection<Table> Tables { get; set; } = new List<Table>();

    public virtual ICollection<Waiter> Waiters { get; set; } = new List<Waiter>();
}

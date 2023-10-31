using System;
using System.Collections.Generic;

namespace WebRestik.Models;

public partial class Menu
{
    public int Id { get; set; }

    public string DishName { get; set; }

    public string Photo { get; set; }

    public string Description { get; set; }
}

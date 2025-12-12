using System;
using System.Collections.Generic;

namespace InventarioMVC.Models;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public DateTime FechaRegistro { get; set; }

    public bool Activo { get; set; }
}

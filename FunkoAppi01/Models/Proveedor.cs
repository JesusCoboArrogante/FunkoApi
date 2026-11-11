namespace FunkoAppi01.Models;

public record Proveedor
{
    public string Id               {get; set; }
    public String Nombre         {get; set; } = string.Empty;
    public long Telefono         {get; set; }
    public bool      IsDeleted   { get; set; }
    public DateTime  CreatedAt   { get; set; }
    public DateTime? UpdatedAt   { get; set; }
}
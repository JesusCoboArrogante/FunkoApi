namespace FunkoAppi01.Models;

public record Categoria
{
   public long Id               {get; set; }
   public String Nombre         { get; set; } = string.Empty;
   public bool      IsDeleted   { get; set; }
   public DateTime  CreatedAt   { get; set; }
   public DateTime? UpdatedAt   { get; set; }
}
namespace FunkoAppi01.Models;

public record Funko
{
    public long      Id          { get; set; }
    public string    Nombre      { get; set; }
    public decimal   Precio      { get; set; }
    public string    Categoria   { get; set; }
    public DateTime  CreateAt    { get; set; }
    public DateTime? UpdateAt    { get; set; }
    public DateTime? DeleteAt    { get; set; }
    
    public bool IsActivo         { get; set; }


}
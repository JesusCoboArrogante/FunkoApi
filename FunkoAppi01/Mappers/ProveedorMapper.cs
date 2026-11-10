using FunkoAppi01.Dto;
using FunkoAppi01.Entity;
using FunkoAppi01.Models;

namespace FunkoAppi01.Mappers;

public static class ProveedorMapper
{
    public static ProveedorDto ToDto(this Proveedor proveedor)
    {
        return new ProveedorDto(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.Telefono
        );
    }

    public static Proveedor ToProveedor(this CreateProveedorDto dto)
    {
        return new Proveedor
        {
            Nombre = dto.Nombre,
            Telefono = dto.Telefono
        };
    }

    public static Proveedor ToProveedor(this UpdateProveedorDto dto)
    {
        return new Proveedor
        {
            Nombre = dto.Nombre,
            Telefono = dto.Telefono
        };
    }

}
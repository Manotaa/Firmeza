using AutoMapper;
using Firmeza.Application.DTOs;
using Firmeza.Domain.Entities;

namespace Firmeza.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Producto, ProductoDto>();
        CreateMap<CrearProductoDto, Producto>();
        CreateMap<ActualizarProductoDto, Producto>();

        CreateMap<Cliente, ClienteDto>();
        CreateMap<CrearClienteDto, Cliente>();
        CreateMap<ActualizarClienteDto, Cliente>();

        CreateMap<Venta, VentaDto>()
            .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Nombre : string.Empty))
            .ForMember(dest => dest.DocumentoCliente, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Documento : string.Empty));

        CreateMap<DetalleVenta, DetalleVentaDto>()
            .ForMember(dest => dest.NombreProducto, opt => opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : string.Empty));
    }
}

using Firmeza.Domain.Entities;
using Firmeza.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Firmeza.Infrastructure.Persistence;

public class FirmezaDbContext : IdentityDbContext<UsuarioFirmeza>
{
    public FirmezaDbContext(DbContextOptions<FirmezaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Configuración de Producto (tabla 'productos')
        builder.Entity<Producto>(entity =>
        {
            entity.ToTable("productos");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(p => p.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            entity.Property(p => p.Precio).HasColumnName("precio").HasPrecision(12, 2).IsRequired();
            entity.Property(p => p.Stock).HasColumnName("stock").HasDefaultValue(0).IsRequired();
            entity.HasIndex(p => p.Nombre).HasDatabaseName("idx_productos_nombre");
        });

        // 2. Configuración de Cliente (tabla 'clientes')
        builder.Entity<Cliente>(entity =>
        {
            entity.ToTable("clientes");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(c => c.Documento).HasColumnName("documento").HasMaxLength(20).IsRequired();
            entity.Property(c => c.Nombre).HasColumnName("nombre").HasMaxLength(150).IsRequired();
            entity.HasIndex(c => c.Documento).IsUnique().HasDatabaseName("idx_clientes_documento_unique");
            entity.HasIndex(c => c.Nombre).HasDatabaseName("idx_clientes_nombre");
        });

        // 3. Configuración de Venta (tabla 'ventas')
        builder.Entity<Venta>(entity =>
        {
            entity.ToTable("ventas");
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(v => v.NumeroVenta).HasColumnName("numero_venta").HasMaxLength(50).IsRequired();
            entity.Property(v => v.Fecha).HasColumnName("fecha").HasColumnType("timestamptz").IsRequired();
            entity.Property(v => v.ClienteId).HasColumnName("cliente_id").IsRequired();
            entity.Property(v => v.Subtotal).HasColumnName("subtotal").HasPrecision(12, 2).IsRequired();
            entity.Property(v => v.Iva).HasColumnName("iva").HasPrecision(12, 2).IsRequired();
            entity.Property(v => v.Total).HasColumnName("total").HasPrecision(12, 2).IsRequired();

            entity.HasIndex(v => v.NumeroVenta).IsUnique().HasDatabaseName("idx_ventas_numero_unique");
            entity.HasIndex(v => v.ClienteId).HasDatabaseName("idx_ventas_cliente_id");
            entity.HasIndex(v => v.Fecha).HasDatabaseName("idx_ventas_fecha");

            entity.HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_ventas_cliente");
        });

        // 4. Configuración de DetalleVenta (tabla 'detalles_venta')
        builder.Entity<DetalleVenta>(entity =>
        {
            entity.ToTable("detalles_venta");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(d => d.VentaId).HasColumnName("venta_id").IsRequired();
            entity.Property(d => d.ProductoId).HasColumnName("producto_id").IsRequired();
            entity.Property(d => d.Cantidad).HasColumnName("cantidad").IsRequired();
            entity.Property(d => d.PrecioUnitario).HasColumnName("precio_unitario").HasPrecision(12, 2).IsRequired();
            entity.Property(d => d.Subtotal).HasColumnName("subtotal").HasPrecision(12, 2).IsRequired();

            entity.HasIndex(d => d.VentaId).HasDatabaseName("idx_detalles_venta_venta_id");
            entity.HasIndex(d => d.ProductoId).HasDatabaseName("idx_detalles_venta_producto_id");

            entity.HasOne(d => d.Venta)
                .WithMany(v => v.Detalles)
                .HasForeignKey(d => d.VentaId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_detalles_venta_venta");

            entity.HasOne(d => d.Producto)
                .WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_detalles_venta_producto");
        });
    }
}

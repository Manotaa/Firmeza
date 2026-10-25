export interface Producto {
  id: number;
  nombre: string;
  precio: number;
  stock: number;
}

export interface CartItem {
  producto: Producto;
  cantidad: number;
}

export interface User {
  id: string;
  email: string;
  nombreCompleto: string;
  rol: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  nombreCompleto: string;
  rol: string;
  expira: string;
}

export interface DetalleVentaDto {
  id: number;
  productoId: number;
  nombreProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
}

export interface VentaDto {
  id: number;
  numeroVenta: string;
  fecha: string;
  clienteId: number;
  nombreCliente: string;
  documentoCliente: string;
  subtotal: number;
  iva: number;
  total: number;
  detalles: DetalleVentaDto[];
}

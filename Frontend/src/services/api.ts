import type { AuthResponse, Producto, User, VentaDto } from '../types';

const API_BASE = 'http://localhost:5249/api';

function getAuthHeader(): Record<string, string> {
  const token = localStorage.getItem('firmeza_token');
  return token ? { Authorization: `Bearer ${token}` } : {};
}

export const api = {
  // Autenticación
  async login(email: string, password: string): Promise<AuthResponse> {
    const res = await fetch(`${API_BASE}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ mensaje: 'Error al iniciar sesión' }));
      throw new Error(err.mensaje || 'Credenciales incorrectas');
    }
    const data: AuthResponse = await res.json();
    localStorage.setItem('firmeza_token', data.token);
    localStorage.setItem('firmeza_user', JSON.stringify(data));
    return data;
  },

  async register(nombreCompleto: string, email: string, password: string): Promise<AuthResponse> {
    const res = await fetch(`${API_BASE}/auth/register`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ nombreCompleto, email, password }),
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ mensaje: 'Error al registrarse' }));
      throw new Error(err.mensaje || 'Error al crear la cuenta');
    }
    const data: AuthResponse = await res.json();
    localStorage.setItem('firmeza_token', data.token);
    localStorage.setItem('firmeza_user', JSON.stringify(data));
    return data;
  },

  async getCurrentUser(): Promise<User | null> {
    const token = localStorage.getItem('firmeza_token');
    if (!token) return null;

    try {
      const res = await fetch(`${API_BASE}/auth/me`, {
        headers: getAuthHeader(),
      });
      if (!res.ok) {
        this.logout();
        return null;
      }
      return await res.json();
    } catch {
      return null;
    }
  },

  logout() {
    localStorage.removeItem('firmeza_token');
    localStorage.removeItem('firmeza_user');
  },

  // Productos
  async getProductos(buscar?: string): Promise<Producto[]> {
    const url = new URL(`${API_BASE}/productos`);
    if (buscar) url.searchParams.append('buscar', buscar);

    const res = await fetch(url.toString());
    if (!res.ok) throw new Error('Error al cargar productos');
    return await res.json();
  },

  // Ventas / Compras
  async crearVenta(clienteId: number, items: { productoId: number; cantidad: number }[]): Promise<VentaDto> {
    const res = await fetch(`${API_BASE}/ventas`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...getAuthHeader(),
      },
      body: JSON.stringify({ clienteId, items }),
    });
    if (!res.ok) {
      const err = await res.json().catch(() => ({ mensaje: 'Error al procesar la compra' }));
      throw new Error(err.mensaje || 'No se pudo completar la compra');
    }
    return await res.json();
  },

  async descargarReciboPdf(ventaId: number, numeroVenta: string): Promise<void> {
    const res = await fetch(`${API_BASE}/ventas/${ventaId}/recibo`, {
      headers: getAuthHeader(),
    });
    if (!res.ok) throw new Error('No se pudo descargar el recibo PDF');

    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `Recibo-${numeroVenta}.pdf`;
    document.body.appendChild(a);
    a.click();
    a.remove();
    window.URL.revokeObjectURL(url);
  },
};

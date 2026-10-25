import { useState, useEffect } from 'react';
import { Navbar } from './components/Navbar';
import { ProductCard } from './components/ProductCard';
import { CartModal } from './components/CartModal';
import { AuthModal } from './components/AuthModal';
import { OrderConfirmationModal } from './components/OrderConfirmationModal';
import { api } from './services/api';
import type { CartItem, Producto, User, VentaDto } from './types';
import { Search, Sparkles, Building2, ShieldCheck, Truck } from 'lucide-react';

function App() {
  const [user, setUser] = useState<User | null>(null);
  const [productos, setProductos] = useState<Producto[]>([]);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [search, setSearch] = useState('');
  const [loading, setLoading] = useState(true);

  // Modales
  const [isCartOpen, setIsCartOpen] = useState(false);
  const [isAuthOpen, setIsAuthOpen] = useState(false);
  const [isCheckingOut, setIsCheckingOut] = useState(false);
  const [confirmedSale, setConfirmedSale] = useState<VentaDto | null>(null);

  // Cargar usuario inicial
  useEffect(() => {
    const savedUser = localStorage.getItem('firmeza_user');
    if (savedUser) {
      try {
        const u = JSON.parse(savedUser);
        setUser({
          id: u.id || '',
          email: u.email,
          nombreCompleto: u.nombreCompleto,
          rol: u.rol,
        });
      } catch {
        // Ignorar
      }
    }

    api.getCurrentUser().then((currentUser) => {
      if (currentUser) setUser(currentUser);
    });
  }, []);

  // Cargar productos
  const loadProductos = async (query?: string) => {
    try {
      setLoading(true);
      const data = await api.getProductos(query);
      setProductos(data);
    } catch (err) {
      console.error('Error al cargar productos:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      loadProductos(search);
    }, 300);
    return () => clearTimeout(timer);
  }, [search]);

  // Manejo de Carrito
  const handleAddToCart = (producto: Producto) => {
    setCart((prev) => {
      const existing = prev.find((item) => item.producto.id === producto.id);
      if (existing) {
        if (existing.cantidad >= producto.stock) {
          alert(`No hay más existencias disponibles para '${producto.nombre}'.`);
          return prev;
        }
        return prev.map((item) =>
          item.producto.id === producto.id
            ? { ...item, cantidad: item.cantidad + 1 }
            : item
        );
      }
      return [...prev, { producto, cantidad: 1 }];
    });
  };

  const handleUpdateQuantity = (productoId: number, delta: number) => {
    setCart((prev) =>
      prev
        .map((item) => {
          if (item.producto.id === productoId) {
            const nuevaCantidad = item.cantidad + delta;
            return nuevaCantidad > 0 ? { ...item, cantidad: nuevaCantidad } : null;
          }
          return item;
        })
        .filter(Boolean) as CartItem[]
    );
  };

  const handleRemoveItem = (productoId: number) => {
    setCart((prev) => prev.filter((item) => item.producto.id !== productoId));
  };

  // Checkout
  const handleCheckout = async () => {
    if (!user) {
      alert('Debes iniciar sesión para realizar una compra.');
      setIsAuthOpen(true);
      return;
    }

    try {
      setIsCheckingOut(true);
      const items = cart.map((i) => ({
        productoId: i.producto.id,
        cantidad: i.cantidad,
      }));

      // ClienteId por defecto: 1 (poblado en seeder o según usuario)
      const venta = await api.crearVenta(1, items);

      setCart([]);
      setIsCartOpen(false);
      setConfirmedSale(venta);
      // Recargar catálogo para actualizar stock
      loadProductos(search);
    } catch (err: any) {
      alert('Error en la compra: ' + err.message);
    } finally {
      setIsCheckingOut(false);
    }
  };

  const handleLogout = () => {
    api.logout();
    setUser(null);
  };

  const totalCartItems = cart.reduce((sum, item) => sum + item.cantidad, 0);

  return (
    <div className="app-container">
      <Navbar
        user={user}
        totalCartItems={totalCartItems}
        onOpenCart={() => setIsCartOpen(true)}
        onOpenAuth={() => setIsAuthOpen(true)}
        onLogout={handleLogout}
      />

      {/* Hero Banner */}
      <section className="hero-banner">
        <div className="hero-content">
          <div className="hero-tag">
            <Sparkles size={16} />
            <span>Firmeza en cada estructura</span>
          </div>
          <h2>La Solución Integral en Materiales de Construcción</h2>
          <p>
            Gestiona tus pedidos con rapidez, seguridad y obtén tu comprobante de compra con cálculo
            detallado de IVA de forma inmediata.
          </p>

          <div className="hero-features">
            <div className="feature-item">
              <Building2 size={20} />
              <span>Calidad Certificada</span>
            </div>
            <div className="feature-item">
              <Truck size={20} />
              <span>Despacho Oportuno</span>
            </div>
            <div className="feature-item">
              <ShieldCheck size={20} />
              <span>Compras Seguras (JWT)</span>
            </div>
          </div>
        </div>
      </section>

      {/* Catálogo de Productos */}
      <main className="catalog-container">
        <div className="catalog-header">
          <div>
            <h2>Catálogo de Productos</h2>
            <p>Selecciona los productos y cantidades para tu proyecto</p>
          </div>

          <div className="search-bar">
            <Search size={20} className="search-icon" />
            <input
              type="text"
              placeholder="Buscar por nombre (ej. Cemento, Varilla)..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>

        {loading ? (
          <div className="loading-state">
            <div className="spinner"></div>
            <p>Cargando catálogo de productos...</p>
          </div>
        ) : productos.length === 0 ? (
          <div className="empty-state">
            <p>No se encontraron productos disponibles.</p>
            <span>Verifica que la API del backend esté en ejecución.</span>
          </div>
        ) : (
          <div className="products-grid">
            {productos.map((producto) => (
              <ProductCard
                key={producto.id}
                producto={producto}
                onAddToCart={handleAddToCart}
              />
            ))}
          </div>
        )}
      </main>

      {/* Modales */}
      <CartModal
        isOpen={isCartOpen}
        onClose={() => setIsCartOpen(false)}
        cart={cart}
        onUpdateQuantity={handleUpdateQuantity}
        onRemoveItem={handleRemoveItem}
        onCheckout={handleCheckout}
        isCheckingOut={isCheckingOut}
      />

      <AuthModal
        isOpen={isAuthOpen}
        onClose={() => setIsAuthOpen(false)}
        onSuccess={(loggedUser) => setUser(loggedUser)}
      />

      <OrderConfirmationModal
        isOpen={Boolean(confirmedSale)}
        onClose={() => setConfirmedSale(null)}
        venta={confirmedSale}
      />
    </div>
  );
}

export default App;

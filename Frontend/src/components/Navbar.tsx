import React from 'react';
import { ShoppingCart, User as UserIcon, LogOut, HardHat } from 'lucide-react';
import type { User } from '../types';

interface NavbarProps {
  user: User | null;
  totalCartItems: number;
  onOpenCart: () => void;
  onOpenAuth: () => void;
  onLogout: () => void;
}

export const Navbar: React.FC<NavbarProps> = ({
  user,
  totalCartItems,
  onOpenCart,
  onOpenAuth,
  onLogout,
}) => {
  return (
    <header className="navbar">
      <div className="navbar-container">
        <div className="logo-section">
          <HardHat className="logo-icon" size={32} />
          <div>
            <h1 className="logo-title">FIRMEZA</h1>
            <p className="logo-subtitle">Materiales & Construcción</p>
          </div>
        </div>

        <div className="nav-actions">
          {user ? (
            <div className="user-badge">
              <UserIcon size={18} />
              <span className="user-name">{user.nombreCompleto}</span>
              <span className="user-role">({user.rol})</span>
              <button onClick={onLogout} className="btn-logout" title="Cerrar sesión">
                <LogOut size={16} />
                <span>Salir</span>
              </button>
            </div>
          ) : (
            <button onClick={onOpenAuth} className="btn-primary">
              <UserIcon size={18} />
              <span>Iniciar Sesión / Registro</span>
            </button>
          )}

          <button onClick={onOpenCart} className="btn-cart">
            <ShoppingCart size={22} />
            <span>Carrito</span>
            {totalCartItems > 0 && <span className="cart-badge">{totalCartItems}</span>}
          </button>
        </div>
      </div>
    </header>
  );
};

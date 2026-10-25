import React from 'react';
import { X, Trash2, Plus, Minus, ShoppingBag, ArrowRight } from 'lucide-react';
import type { CartItem } from '../types';

interface CartModalProps {
  isOpen: boolean;
  onClose: () => void;
  cart: CartItem[];
  onUpdateQuantity: (productoId: number, delta: number) => void;
  onRemoveItem: (productoId: number) => void;
  onCheckout: () => void;
  isCheckingOut: boolean;
}

export const CartModal: React.FC<CartModalProps> = ({
  isOpen,
  onClose,
  cart,
  onUpdateQuantity,
  onRemoveItem,
  onCheckout,
  isCheckingOut,
}) => {
  if (!isOpen) return null;

  const subtotal = cart.reduce((sum, item) => sum + item.producto.precio * item.cantidad, 0);
  const iva = subtotal * 0.19; // 19% IVA (Requisito M6.3)
  const total = subtotal + iva;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-card cart-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <div className="modal-title">
            <ShoppingBag size={24} />
            <h2>Tu Carrito de Compras</h2>
          </div>
          <button onClick={onClose} className="btn-close" aria-label="Cerrar">
            <X size={20} />
          </button>
        </div>

        <div className="modal-body">
          {cart.length === 0 ? (
            <div className="empty-cart">
              <ShoppingBag size={64} className="empty-icon" />
              <p>Tu carrito está vacío.</p>
              <span>¡Explora nuestro catálogo y añade los materiales que necesitas!</span>
            </div>
          ) : (
            <div className="cart-items-list">
              {cart.map((item) => (
                <div key={item.producto.id} className="cart-item">
                  <div className="cart-item-details">
                    <h4>{item.producto.nombre}</h4>
                    <span className="cart-item-unit-price">
                      ${item.producto.precio.toLocaleString('es-CO', { minimumFractionDigits: 2 })} c/u
                    </span>
                  </div>

                  <div className="cart-item-actions">
                    <div className="quantity-controls">
                      <button
                        onClick={() => onUpdateQuantity(item.producto.id, -1)}
                        className="btn-qty"
                      >
                        <Minus size={14} />
                      </button>
                      <span className="qty-value">{item.cantidad}</span>
                      <button
                        onClick={() => onUpdateQuantity(item.producto.id, 1)}
                        disabled={item.cantidad >= item.producto.stock}
                        className="btn-qty"
                      >
                        <Plus size={14} />
                      </button>
                    </div>

                    <span className="cart-item-subtotal">
                      ${(item.producto.precio * item.cantidad).toLocaleString('es-CO', { minimumFractionDigits: 2 })}
                    </span>

                    <button
                      onClick={() => onRemoveItem(item.producto.id)}
                      className="btn-remove"
                      title="Eliminar producto"
                    >
                      <Trash2 size={16} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {cart.length > 0 && (
          <div className="cart-footer">
            <div className="totals-summary">
              <div className="totals-row">
                <span>Subtotal:</span>
                <span>${subtotal.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
              </div>
              <div className="totals-row">
                <span>IVA (19%):</span>
                <span>${iva.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
              </div>
              <div className="totals-row totals-highlight">
                <span>TOTAL A PAGAR:</span>
                <span>${total.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
              </div>
            </div>

            <button
              onClick={onCheckout}
              disabled={isCheckingOut}
              className="btn-checkout"
            >
              <span>{isCheckingOut ? 'Procesando compra...' : 'Finalizar Compra y Generar Recibo'}</span>
              <ArrowRight size={20} />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};

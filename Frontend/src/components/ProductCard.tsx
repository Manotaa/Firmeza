import React from 'react';
import { Plus, PackageCheck, AlertCircle } from 'lucide-react';
import type { Producto } from '../types';

interface ProductCardProps {
  producto: Producto;
  onAddToCart: (producto: Producto) => void;
}

export const ProductCard: React.FC<ProductCardProps> = ({ producto, onAddToCart }) => {
  const tieneStock = producto.stock > 0;

  return (
    <div className={`product-card ${!tieneStock ? 'out-of-stock' : ''}`}>
      <div className="product-image-placeholder">
        <PackageCheck size={48} className="product-icon" />
      </div>

      <div className="product-info">
        <h3 className="product-name">{producto.nombre}</h3>
        <div className="product-meta">
          <span className="product-price">
            ${producto.precio.toLocaleString('es-CO', { minimumFractionDigits: 2 })}
          </span>
          <span className={`product-stock ${tieneStock ? 'in-stock' : 'no-stock'}`}>
            {tieneStock ? `${producto.stock} disponibles` : 'Sin stock'}
          </span>
        </div>
      </div>

      <button
        onClick={() => onAddToCart(producto)}
        disabled={!tieneStock}
        className="btn-add-cart"
      >
        {tieneStock ? (
          <>
            <Plus size={18} />
            <span>Agregar al Carrito</span>
          </>
        ) : (
          <>
            <AlertCircle size={18} />
            <span>Agotado</span>
          </>
        )}
      </button>
    </div>
  );
};

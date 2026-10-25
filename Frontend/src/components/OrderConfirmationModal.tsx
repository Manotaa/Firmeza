import React from 'react';
import { CheckCircle2, Download, X } from 'lucide-react';
import type { VentaDto } from '../types';
import { api } from '../services/api';

interface OrderConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  venta: VentaDto | null;
}

export const OrderConfirmationModal: React.FC<OrderConfirmationModalProps> = ({
  isOpen,
  onClose,
  venta,
}) => {
  if (!isOpen || !venta) return null;

  const handleDownloadPdf = async () => {
    try {
      await api.descargarReciboPdf(venta.id, venta.numeroVenta);
    } catch (err: any) {
      alert('Error al descargar el PDF: ' + err.message);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-card receipt-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <div className="success-banner">
            <CheckCircle2 size={40} className="success-icon" />
            <div>
              <h2>¡Compra Realizada con Éxito!</h2>
              <p>Tu comprobante de venta ha sido generado.</p>
            </div>
          </div>
          <button onClick={onClose} className="btn-close" aria-label="Cerrar">
            <X size={20} />
          </button>
        </div>

        <div className="receipt-content">
          <div className="receipt-meta">
            <div className="meta-item">
              <span>N° de Venta:</span>
              <strong>{venta.numeroVenta}</strong>
            </div>
            <div className="meta-item">
              <span>Fecha:</span>
              <strong>{new Date(venta.fecha).toLocaleString('es-CO')}</strong>
            </div>
            <div className="meta-item">
              <span>Cliente:</span>
              <strong>{venta.nombreCliente}</strong>
            </div>
          </div>

          <div className="receipt-items-table">
            <div className="table-header">
              <span>Producto</span>
              <span>Cant.</span>
              <span>Total</span>
            </div>
            {venta.detalles.map((d) => (
              <div key={d.id || d.productoId} className="table-row">
                <span>{d.nombreProducto}</span>
                <span>{d.cantidad}</span>
                <span>${d.subtotal.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
              </div>
            ))}
          </div>

          <div className="receipt-totals">
            <div className="totals-row">
              <span>Subtotal:</span>
              <span>${venta.subtotal.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
            </div>
            <div className="totals-row">
              <span>IVA (19%):</span>
              <span>${venta.iva.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
            </div>
            <div className="totals-row total-final">
              <span>Total Pagado:</span>
              <span>${venta.total.toLocaleString('es-CO', { minimumFractionDigits: 2 })}</span>
            </div>
          </div>
        </div>

        <div className="receipt-actions">
          <button onClick={handleDownloadPdf} className="btn-download-pdf">
            <Download size={20} />
            <span>Descargar Comprobante PDF</span>
          </button>
          <button onClick={onClose} className="btn-secondary">
            Continuar Comprando
          </button>
        </div>
      </div>
    </div>
  );
};

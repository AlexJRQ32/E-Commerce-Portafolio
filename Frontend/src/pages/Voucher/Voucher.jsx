import { ButtonRedirect } from '../../components/Button/Button'
import './Voucher.css'

export function Voucher() {
  return (
    <div className="page">
      <section className="section-voucher">
        <div className="voucher-header">
          <div className="voucher-title">
            <h1>Rappi'Doz</h1>
            <p>Payment receipt</p>
          </div>
          <p>Order #1 {/* Este numero debe ser el de la orden */}</p>
          <span>
            {' '}
            08/05/2026 09:23 {/* Aqui la fecha y hora de la transaccion */}{' '}
          </span>
        </div>
        <div className="product-line">
          <span>1 x Double Whopper</span>
          <span>$7</span>
        </div>
        <div className="voucher-body">
          <div className="total-row">
            <span>Subtotal</span>
            <span>$7</span>
          </div>
          <div className="total-row">
            <span>Delivery Fee</span>
            <span>$2</span>
          </div>
          <div className="total-row">
            <span>Service Fee</span>
            <span>$1.5</span>
          </div>
        </div>
        <div className="total">
          <h2>Total</h2>
          <h2>$10.5</h2>
        </div>
        <div className="voucher-footer">
          <p>¡Thanks for your order!</p>
          <i className='fa-solid fa-4x fa-qrcode'></i>
          <ButtonRedirect title={'VIEW TRACKING ON HOME SCREEN'} site={'home'} icon={'route'} />
          <a href="/cart">GO BACK TO CART</a>
        </div>
      </section>
    </div>
  )
}

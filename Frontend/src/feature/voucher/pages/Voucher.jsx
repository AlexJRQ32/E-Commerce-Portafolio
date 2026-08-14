import { ButtonRedirect } from '../../../components/Button/Button'
import { useMappedObjects } from '../../../common/hooks/useMappedObjects'
import './Voucher.css'

export function Voucher() {
  const { orders, loading } = useMappedObjects()

  // Mostrar la última orden (la más reciente)
  const order = orders.length > 0 ? orders[orders.length - 1] : null

  if (loading) {
    return (
      <div className="page">
        <section className="section-voucher">
          <p>Loading order...</p>
        </section>
      </div>
    )
  }

  if (!order) {
    return (
      <div className="page">
        <section className="section-voucher">
          <p>No orders found.</p>
          <a href="/cart">GO BACK TO CART</a>
        </section>
      </div>
    )
  }

  // Calcular subtotal sumando precio * cantidad de cada item
  const subtotal = order.items
    ? order.items.reduce((sum, item) => sum + (item.price ?? 0) * (item.quantity ?? 1), 0)
    : order.total ?? 0

  const deliveryFee = order.deliveryFee ?? 2
  const serviceFee = order.serviceFee ?? 1.5
  const total = order.total ?? subtotal + deliveryFee + serviceFee

  return (
    <div className="page">
      <section className="section-voucher">
        <div className="voucher-header">
          <div className="voucher-title">
            <h1>Rappi'Doz</h1>
            <p>Payment receipt</p>
          </div>
          <p>Order #{order.id}</p>
          <span>{order.date} {order.time}</span>
        </div>

        {/* Items de la orden */}
        {order.items && order.items.length > 0
          ? order.items.map((item, index) => (
              <div className="product-line" key={index}>
                <span>{item.quantity ?? 1} x {item.name}</span>
                <span>${((item.price ?? 0) * (item.quantity ?? 1)).toFixed(2)}</span>
              </div>
            ))
          : (
              <div className="product-line">
                <span>Order items not available</span>
              </div>
            )
        }

        <div className="voucher-body">
          <div className="total-row">
            <span>Subtotal</span>
            <span>${subtotal.toFixed(2)}</span>
          </div>
          <div className="total-row">
            <span>Delivery Fee</span>
            <span>${deliveryFee.toFixed(2)}</span>
          </div>
          <div className="total-row">
            <span>Service Fee</span>
            <span>${serviceFee.toFixed(2)}</span>
          </div>
        </div>

        <div className="total">
          <h2>Total</h2>
          <h2>${typeof total === 'number' ? total.toFixed(2) : total}</h2>
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

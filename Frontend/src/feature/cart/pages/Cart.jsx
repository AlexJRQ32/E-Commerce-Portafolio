import './Cart.css'
import { NavLink } from 'react-router-dom'
import { Select } from '../../../components/Select/Select'
import { ButtonAction, ButtonRedirect } from '../../../components/Button/Button'
import { useMappedObjects } from '../../../common/hooks/useMappedObjects'
import { CouponCard } from '../../../components/Coupons/Coupons'

export function Cart() {
  const { addresses, paymentMethods, coupons } = useMappedObjects()

  return (
    <div className="page">
      <section className="cart-section">
        <div className="cart-header">
          <h1>
            <i className="fa-solid fa-shopping-basket"></i>
            MY ORDER
          </h1>
          <NavLink to="/search">
            <i className="fa-solid fa-chevron-left"></i>
            CONTINUE SHOPPING
          </NavLink>
        </div>
        <div className="cart-body">
          <div className="cart-items">Your cart is currently empty.</div>
          <div className="cart-details">
            <div className="cart-detail">
              <span>
                <i className="fa-solid fa-location-dot"></i>
                DELIVERY ADDRESS
              </span>
              <div className="form">
                <Select name={'addresses'} parameters={addresses} title={'location'} />
                <ButtonAction icon={'xmark'} className={'delete'} />
              </div>
              <div className="link-add">
                <NavLink to="/search">
                  <i className='fa-solid fa-add'></i>
                  Manage addresses
                </NavLink>
              </div>
            </div>
            <div className="spacer"></div>
            <div className="cart-detail">
              <span>
                <i className='fa-solid fa-credit-card'></i>
                PAYMENT METHOD
              </span>
              <div className="form">
                <Select name={'payment-methods'} parameters={paymentMethods} title={'payment method'} />
              </div>
            </div>
            <div className="spacer"></div>
            <div className="cart-detail">
              <span>
                <i className='fa-solid fa-ticket'></i>
                CUPON WALLET
              </span>
              <div className="container">
                {
                  coupons.length === 0 ? (<p>No benefits available.</p>) : (
                    <CouponCard coupons={coupons} />
                  )
                }
              </div>
            </div>
            <div className="spacer"></div>
            <div className="cart-detail">
              <div className="container-row">
                <span> Subtotal </span>
                <strong >0$ </strong>
              </div>
              <div className="container-row">
                <span> Delivery fee </span>
                <strong>0$</strong>
              </div>
            </div>
            <div className="spacer"></div>
            <div className="cart-detail">
              <div className="container-row">
                <h2>TOTAL</h2>
                <h2>0$</h2>
              </div>
              <ButtonRedirect title={'CONFIRM ORDER'} className={'confirm'} site={'voucher'} />
            </div>
          </div>
        </div>
      </section>
    </div>
  )
}

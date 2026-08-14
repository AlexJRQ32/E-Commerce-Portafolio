import { ButtonAction } from '../Button/Button'
import './Coupons.css'

export function CouponCard({ coupons }) {
  return (
    <ul className="coupons">
      {coupons.map((coupon) => (
        <li key={coupon.id}>
          <div className="coupon-card">
            <span>
              <i className="fa-solid fa-tag"></i>
              {coupon.name}
            </span>
            {/\d/.test(coupon.discount) ? (
              <p>{coupon.discount}% OFF</p>
            ) : (
              <p>{coupon.discount}</p>
            )}
          </div>
        </li>
      ))}
    </ul>
  )
}

export function CouponTicket({ coupons, className, onclick }) {
  const classNameAcepted = 'coupon-buttons'
  return (
    <ul className="grid-body">
      {coupons.map(coupon => (
        <li key={coupon.id}>
          <div className="coupon-ticket">
            <div class="pin-seguro"></div>
            <div className="coupon-header">
              <p>RappiDoz Ticket</p>
              {coupon.isPercentage ? <h1>{coupon.discount}%</h1> : <h1>{coupon.discount}</h1>}
              <span>OFF</span>
            </div>
            <div className="coupon-body">
              <span>
                <strong>{coupon.code}</strong>
                <p>{coupon.description}</p>
              </span>
              {className === classNameAcepted ? (
                <div className={className}>
                  <ButtonAction icon={'pencil'} className={'edit'} onclick={onclick} />
                  <ButtonAction icon={'trash'} className={'delete'} onclick={onclick} />
                </div>
              ) : (
                <div className={className}></div>
              )}

              <p>{coupon.stock} remaining</p>
            </div>
          </div>
        </li>
      ))}
    </ul>
  )
}

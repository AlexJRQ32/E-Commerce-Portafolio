import { CouponTicket } from '../../components/Coupons/Coupons'
import { useMappedObjects } from '../../hooks/useMappedObjects'
import './Coupons.css'

export function Coupons() {
  const { coupons } = useMappedObjects()

  return (
    <div className="page">
      <div className="coupons-section">
        <div className="coupons-header">
          <h1>Benefits Summary</h1>
        </div>
        <div className="coupons-body">
          <CouponTicket coupons={coupons} />
        </div>
      </div>
    </div>
  )
}

import { useMappedObjects } from '../../hooks/useMappedObjects'
import { OrderSimpleCard } from '../../components/Cards/Cards'
import { ButtonRedirect } from "../../components/Button/Button"

export function OrderHistory() {
  const { orders } = useMappedObjects()

  return(
    <div className="page">
      <div className="order-section">
        <div className="order-header">
          <span>
            <i className="fa-solid fa-receipt"></i>
            <h1>My Vouchers</h1>
          </span>
        </div>
        {
          orders ?
          <div className="order-body">
            <OrderSimpleCard orders={orders} />
          </div>
          :
          <div className="order-body">
            <i className="fa-solid fa-box-open fa-4x icon-empty"></i>
            <strong>You don't have any registered orders yet</strong>
            <ButtonRedirect title={'Explore restaurants'} site={'/search'}/>
          </div>
        }
      </div>
    </div>
  )
}

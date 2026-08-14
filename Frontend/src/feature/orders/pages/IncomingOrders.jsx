import './IncomingOrders.css'
import { useMappedObjects } from '../../../common/hooks/useMappedObjects'
import { OrderCard } from '../../../components/Cards/Cards'

export function IncomingOrders() {
  const { restaurants, orders } = useMappedObjects()
  const restaurantNames = restaurants.map((restaurant) => restaurant.tradeName)
  const selectedRestaurant = restaurantNames.find(
    (name) => name === 'Taco Bell'
  )

  return (
    <div className="page">
      <div className="order-section">
        <div className="order-header">
          <span>
            <i className="fa-solid fa-shopping-bag"></i>
            <h1>Incoming Orders</h1>
          </span>
          <p>{selectedRestaurant || 'Restaurant not found'}</p>
        </div>
        {orders ? (
          <div className="order-body">
            <OrderCard orders={orders} />
          </div>
        ) : (
          <p className="empty">Your quaue is empty</p>
        )}
      </div>
    </div>
  )
}

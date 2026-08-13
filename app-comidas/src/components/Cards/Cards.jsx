import './Cards.css'
import { ButtonAction, ButtonRedirect } from '../Button/Button'
import { NavLink } from 'react-router-dom'

export function CategoryCard({ categories }) {
  return (
    <ul className="categories">
      {categories.map(categorie => (
        <li key={categorie.id}>
          <NavLink to="/search" className="categorie-card">
            <i className={`fa-solid fa-${categorie.icon}`}></i>
            <h3>{categorie.name}</h3>
            {categorie.subtitle ?? <p>{categorie.subtitle}</p>}
          </NavLink>
        </li>
    ))}
    </ul>
  )
}

export function RoleCard({ roles }) {
  return (
    <ul className="roles">
      {roles.map(role => (
        <li key={role.id}>
          <NavLink to={role.site} className="role-card">
            <i className={`fa-solid fa-${role.icon} fa-3x`}></i>
            <div className="role-titles">
              <h2>{role.name}</h2>
              <p>{role.subtitle}</p>
            </div>
          </NavLink>
        </li>
    ))}
    </ul>
  )
}

export function RestaurantCard({ restaurants }) {
  return (
    <ul className="restaurants">
      {restaurants.map(restaurant => (
        <li key={restaurant.id}>
          <NavLink to="/search" className="restaurant-card">
            <img src={restaurant.img} alt={restaurant.tradeName} />
            <h3>{restaurant.tradeName}</h3>
            <div className="other-info">
              <span>
                <i className="fa-solid fa-clock" />
                {restaurant.openingTime} - {restaurant.closingTime}
              </span>
              <span>
                <i className="fa-solid fa-star" />
                {restaurant.rating}
              </span>
            </div>
          </NavLink>
        </li>
      ))}
    </ul>
  )
}

export function RestaurantLargeCard({ restaurants }) {
  return (
    <ul className='show-restaurants'>
      {restaurants.map(restaurant => (
        <li key={restaurant.id}>
          <div className="large-card">
            <img src={restaurant.img} alt={restaurant.tradeName} />
            <div className="card-info">
              <div className="content">
                <div id="top">
                  <h3>{restaurant.tradeName}</h3>
                  <span>
                    <i className="fa-solid fa-location-dot" />
                    {restaurant.address}
                  </span>
                </div>
                {restaurant.isOpen ? (
                  <span className="status is-open">OPEN</span>
                ) : (
                  <span className="status is-closed">CLOSED</span>
                )}
              </div>
              <div className="content">
                <div id="bot">
                  <span>
                    <i className="fa-regular fa-clock"></i>
                    {restaurant.openingTime} - {restaurant.closingTime}
                  </span>
                  <p>
                    {restaurant.deliveryFee} delivery • {restaurant.deliveryTime}
                  </p>
                </div>
                <ButtonRedirect title={'View menu'} />
              </div>
            </div>
          </div>
        </li>
      ))}
    </ul>
  )
}

export function OrderCard({ orders }) {
  
  const statusClass = ({ status }) => {
    if (status === 'DELIVERED') {
      return 'completed'
    } else if (status === 'PENDING') {
      return 'pending'
    } else {
      return 'cancelled'
    }
  }

  return (
    <ul className='orders'>
      {orders.map(order => (
        <li key={order.id}>
          <div className="order-card">
            <div className="card-header">
              <div className="left-side">
                <h2>Order #{order.id}</h2>
                <p>
                  <i className="fa-solid fa-calendar-days"></i>
                  {order.date}
                  <span>
                    <i className="fa-regular fa-clock"></i>
                    {order.time}
                  </span>
                </p>
                <p>
                  <i className="fa-solid fa-user"></i>
                  {order.customer}
                </p>
                <p>
                  <i className="fa-solid fa-credit-card"></i>
                  {order.paymentMethod}
                </p>
              </div>
              <div className="right-side">
                <span className={statusClass({ status: order.status})}>{order.status}</span>
                <h3>${order.total}</h3>
              </div>
            </div>
            <div className="card-items">
              <i className="fa-solid fa-circle-dot"></i>
              {order.items.map((item) => (
                <span className="order-item" key={item.name}>
                  {item.quantity}x {item.name} (${item.price})
                </span>
              ))}
            </div>
          </div>
        </li>
      ))}
    </ul>
  )
}

export function OrderSimpleCard({ orders }) {
  const statusClass = ({ status }) => {
    if (status === 'DELIVERED') {
      return 'completed'
    } else if (status === 'PENDING') {
      return 'pending'
    } else {
      return 'cancelled'
    }
  }

  return (
    <ul className='orders'>
      {orders.map(order => (
        <li key={order.id}>
          <div className="order-card" >
            <div className="card-header">
              <div className="left-side">
                <h2>Order #{order.id}</h2>
                <p>
                  <i className="fa-solid fa-calendar-days"></i> {order.date}
                  <i className="fa-regular fa-clock"></i> {order.time}
                </p>
              </div>
              <div className="right-side">
                <span className={statusClass({ status: order.status })}>{order.status}</span>
                <h3>${order.total}</h3>
                <ButtonRedirect
                  icon={'star'}
                  title={'Rate'}
                />
              </div>
            </div>
          </div>
        </li>
      ))}
    </ul>
  )
}

export function StatCard({ statcards }) {
  return(
    <ul className="stats-cards">
      {statcards.map(statcard => (
        <li key={statcard.id}>
          <NavLink to={statcard.site} className={"stat-card"}>
            <div>
              <span>
                <i className={`fa-solid fa-${statcard.icon}`}></i>
              </span>
              <p>{statcard.title} {statcard.value}</p>
            </div>
          </NavLink>
        </li>
      ))}
    </ul>
  )
}

export function CrudCard({ img, name, attribute, className, icon, onclick}) {
  return(
    <div className={`crud-card ${className}`}>
      <img src={img} alt={name} />
      <span>
        <p>{name}</p>
        <strong><i className={`fa-solid fa-${icon}`}></i> {attribute}</strong>
      </span>
      <div>
        <ButtonAction icon={'pencil'} className={'edit'} onclick={onclick} />
        <ButtonAction icon={'trash'} className={'delete'}/>
      </div>
    </div>
  )
}
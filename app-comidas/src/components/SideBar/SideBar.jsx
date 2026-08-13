import { NavLink } from 'react-router-dom'
import './SideBar.css'
import { ButtonRedirect } from '../Button/Button'

export function SideBar() {
  const userData = {
    name: 'John Doe',
    rol: 'Customer',
  }

  return (
    <section className="sidebar-section">
      <nav className="sidebar">
        <div className="sidebar-header">
          <h1>RAPPI'DOZ</h1>
          <p>GOLD</p>
        </div>
        <ul>
          <li>
            <NavLink to="/dashboard/overview">
              <i className="fa-solid fa-house"></i>
              Overview
            </NavLink>
          </li>
          <li>
            <NavLink to="/dashboard/my-menu">
              <i className="fa-solid fa-utensils"></i>
              My menu
            </NavLink>
          </li>
          <li>
            <NavLink to="/dashboard/restaurants">
              <i className="fa-solid fa-store"></i>
              Restaurants
            </NavLink>
          </li>
          <li>
            <NavLink to="/dashboard/users">
              <i className="fa-solid fa-user-shield"></i>
              Users
            </NavLink>
          </li>
          <li>
            <NavLink to="/dashboard/coupons-dashboard">
              <i className="fa-solid fa-ticket"></i>
              Coupons
            </NavLink>
          </li>
        </ul>
        <div className="exit-section">
          <p>Logged in as:</p>
          <strong>{userData.name}</strong>
          <ButtonRedirect
            icon={'power-off'}
            title={'Exit'}
            site={'home'}
            className={'exit'}
          />
        </div>
      </nav>
    </section>
  )
}

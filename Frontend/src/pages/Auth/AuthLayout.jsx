import './AuthLayout.css'
import { Outlet } from 'react-router-dom'

export function AuthLayout() {
  return (
    <div className='auth-layout'>
      <div className="auth-container">
        <Outlet />
      </div>
    </div>
  )
}
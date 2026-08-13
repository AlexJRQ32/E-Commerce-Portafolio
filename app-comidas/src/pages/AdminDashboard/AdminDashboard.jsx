import { Outlet } from 'react-router-dom'
import { SideBar } from '../../components/SideBar/SideBar'
import './AdminDashbord.css'

export function AdminDashboard() {
  return (
    <div className='dashboard-layout'>
      <SideBar />
      <main className="dashboard-content">
        <Outlet />
      </main>
    </div>
  )
}
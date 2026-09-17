import './App.css'
import { Routes, Route, Navigate } from 'react-router-dom'
import { Home } from './feature/home/pages/Home'
import { Search } from './feature/search/pages/Search'
import { Cart } from './feature/cart/pages/Cart'
import { IncomingOrders } from './feature/orders/pages/IncomingOrders'
import { OrderHistory } from './feature/orders/pages/OrderHistory'
import { Coupons } from './feature/coupons/pages/Coupons'
import { AdminDashboard } from './feature/dashboard/pages/AdminDashboard'
import { UserLayout } from './UserLayout'
import { OverviewDashboard } from './feature/dashboard/pages/Dashboard Pages/Overview'
import { MyMenuDashboard } from './feature/dashboard/pages/Dashboard Pages/MyMenu'
import { RestaurantsDashboard } from './feature/dashboard/pages/Dashboard Pages/Restaurants'
import { UsersDashboard } from './feature/dashboard/pages/Dashboard Pages/Users'
import { CouponsDashboard } from './feature/dashboard/pages/Dashboard Pages/CouponsDashboard'
import { Voucher } from './feature/voucher/pages/Voucher'
import { AuthLayout } from './feature/auth/pages/AuthLayout'
import { SignIn } from './feature/auth/pages/SignIn'
import { SignUp } from './feature/auth/pages/SignUp'
import { ChooseRole } from './feature/auth/pages/ChooseRole'
import { RegisterBusiness } from './feature/auth/pages/RegisterBusiness'
import { ModalProvider } from './common/context/modal'
import { AuthProvider, useAuth } from './common/context/AuthContext'

function ProtectedRoute({ children, allowedRoles }) {
  const { user, loading } = useAuth()
  if (loading) return <div className="loading">Cargando...</div>
  if (!user) return <Navigate to="/auth/sign-in" replace />
  if (allowedRoles && !allowedRoles.includes(user.role)) {
    return <Navigate to="/home" replace />
  }
  return children
}

function App() {
  return (
    <AuthProvider>
      <ModalProvider>
        <div className="app">
          <Routes>
            <Route path="/" element={<Navigate to="/home" replace />} />
            
            <Route element={<UserLayout />}>
              <Route path="/home" element={<Home />} />
              <Route path="/search" element={<Search />} />
              <Route path="/cart" element={<ProtectedRoute><Cart /></ProtectedRoute>} />
              <Route path="/incoming-orders" element={
                <ProtectedRoute allowedRoles={['Business']}><IncomingOrders /></ProtectedRoute>
              } />
              <Route path="/order-history" element={<ProtectedRoute><OrderHistory /></ProtectedRoute>} />
              <Route path="/coupons" element={<ProtectedRoute><Coupons /></ProtectedRoute>} />
              <Route path='/voucher' element={<ProtectedRoute><Voucher /></ProtectedRoute>} />
            </Route>

            <Route path="/dashboard" element={
              <ProtectedRoute allowedRoles={['Admin', 'Business']}>
                <AdminDashboard />
              </ProtectedRoute>
            }>
              <Route index element={<Navigate to="overview" replace />} />
              <Route path="overview" element={<ProtectedRoute allowedRoles={['Admin', 'Business']}><OverviewDashboard /></ProtectedRoute>} />
              <Route path="my-menu" element={<ProtectedRoute allowedRoles={['Business']}><MyMenuDashboard /></ProtectedRoute>} />
              <Route path="restaurants" element={<ProtectedRoute allowedRoles={['Admin', 'Business']}><RestaurantsDashboard /></ProtectedRoute>} />
              <Route path="users" element={<ProtectedRoute allowedRoles={['Admin']}><UsersDashboard /></ProtectedRoute>} />
              <Route path="coupons-dashboard" element={<ProtectedRoute allowedRoles={['Admin', 'Business']}><CouponsDashboard /></ProtectedRoute>} />
            </Route>

            <Route path="/auth" element={<AuthLayout />}>
              <Route path="sign-in" element={<SignIn />} />
              <Route path="sign-up" element={<SignUp />} />
              <Route path='choose-role' element={<ChooseRole />} />
              <Route path='register-business' element={
                <ProtectedRoute allowedRoles={['Business']}>
                  <RegisterBusiness />
                </ProtectedRoute>
              } />
            </Route>
          </Routes>
        </div>
      </ModalProvider>
    </AuthProvider>
  )
}

export default App

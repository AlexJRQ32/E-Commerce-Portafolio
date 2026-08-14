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

function App() {
  return (
    <div className="app">
      <Routes>
        <Route path="/" element={<Navigate to="/home" replace />} />
        <Route element={<UserLayout />}>
          <Route path="/home" element={<Home />} />
          <Route path="/search" element={<Search />} />
          <Route path="/cart" element={<Cart />} />
          <Route path="/incoming-orders" element={<IncomingOrders />} />
          <Route path="/order-history" element={<OrderHistory />} />
          <Route path="/coupons" element={<Coupons />} />
          <Route path='/voucher' element={<Voucher />} />
        </Route>

        <Route path="/dashboard" element={<AdminDashboard />}>
          <Route index element={<Navigate to="overview" replace />} />
          <Route path="overview" element={<OverviewDashboard />} />
          <Route path="my-menu" element={<MyMenuDashboard />} />
          <Route path="restaurants" element={<RestaurantsDashboard />} />
          <Route path="users" element={<UsersDashboard />} />
          <Route path="coupons-dashboard" element={<CouponsDashboard />} />
        </Route>

        <Route path="/auth" element={<AuthLayout />}>
          <Route path="sign-in" element={<SignIn />} />
          <Route path="sign-up" element={<SignUp />} />
          <Route path='choose-role' element={<ChooseRole />} />
          <Route path='register-business' element={<RegisterBusiness />} />
        </Route>
      </Routes>
    </div>
  )
}

export default App

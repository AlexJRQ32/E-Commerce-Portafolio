import { GetUsers } from '../services/UsersEndpoints'
import { GetCategories } from '../services/GeneralDataEndpoints'
import { GetDishes } from '../services/DishesEndpoints'
import { GetCoupons } from '../services/CouponsEndpoints'
import { GetOrders } from '../services/OrdersEndpoints'
import { GetPaymentMethods } from '../services/GeneralDataEndpoints'
import { GetRestaurants } from '../services/RestaurantsEndpoints'
import { GetUserAddresses } from '../services/UsersEndpoints'
import { GetRoles } from '../services/GeneralDataEndpoints'
import StatCards from '../mocks/statcards.json'

export function useMappedObjects() {
  const mappedUsers = GetUsers().map((user) => ({
    id: user.id,
    email: user.email,
    name: user.name,
    role: user.role,
    img: user.img,
    password: user.password,
    phone: user.phone,
  }))

  const mappedCategories = GetCategories().map((element) => ({
    id: element.id,
    name: element.name,
    icon: element.icon,
    slug: element.slug,
  }))

  const mappedDishes = GetDishes().map((dishe) => ({
    id: dishe.id,
    img: dishe.img,
    category: dishe.category,
    description: dishe.description,
    price: dishe.price,
    name: dishe.name,
  }))

  const mappedCoupons = GetCoupons().map((coupon) => ({
    id: coupon.Id,
    code: coupon.Code,
    title: coupon.Title,
    description: coupon.Description,
    discount: coupon.Discount,
    isPercentage: coupon.IsPercentage,
    expirationDate: coupon.ExpirationDate,
    active: coupon.Active,
    stock: coupon.Stock,
    categoryId: coupon.CategoryId,
  }))

  const mappedOrders = GetOrders().map((order) => ({
    id: order.id,
    restaurant: order.restaurant,
    status: order.status,
    date: order.date,
    time: order.time,
    customer: order.customer,
    paymentMethod: order.paymentMethod,
    items: order.items,
    total: order.total,
  }))

  const mappedPaymentMethods = GetPaymentMethods().map((method) => ({
    id: method.id,
    icon: method.icono,
    name: method.name,
    type: method.tipo,
    description: method.descripcion,
  }))

  const mappedRestaurants = GetRestaurants().map((restaurant) => ({
    id: restaurant.Id,
    tradeName: restaurant.TradeName,
    address: restaurant.Address,
    categoryId: restaurant.CategoryId,
    openingTime: restaurant.OpeningTime,
    closingTime: restaurant.ClosingTime,
    img: restaurant.Img,
    rating: restaurant.Rating,
    isOpen: restaurant.IsOpen,
    deliveryFee: restaurant.DeliveryFee,
    deliveryTime: restaurant.DeliveryTime,
  }))

  const mappedAddresses = GetUserAddresses().map((ubication) => ({
    id: ubication.id,
    name: ubication.name,
  }))

  const mappedRoles = GetRoles().map((role) => ({
    id: role.id,
    name: role.name,
    subtitle: role.subtitle,
    site: role.site,
    icon: role.icon,
  }))

  const mappedStatCards = StatCards.map((statCard) => ({
    id: statCard.id,
    icon: statCard.icon,
    site: statCard.site,
    title: statCard.title,
    value: statCard.value,
  }))

  return {
    users: mappedUsers,
    categories: mappedCategories,
    dishes: mappedDishes,
    coupons: mappedCoupons,
    orders: mappedOrders,
    paymentMethods: mappedPaymentMethods,
    restaurants: mappedRestaurants,
    statCards: mappedStatCards,
    addresses: mappedAddresses,
    roles: mappedRoles,
  }
}

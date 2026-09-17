import { useState, useEffect, useCallback } from 'react'
import { GetUsers } from '../services/UsersEndpoints'
import { GetCategories } from '../services/GeneralDataEndpoints'
import { GetDishes } from '../services/DishesEndpoints'
import { GetCoupons } from '../services/CouponsEndpoints'
import { GetOrders } from '../services/OrdersEndpoints'
import { GetPaymentMethods } from '../services/GeneralDataEndpoints'
import { GetRestaurants } from '../services/RestaurantsEndpoints'
import { GetUserAddresses } from '../services/UsersEndpoints'
import { GetRoles } from '../services/GeneralDataEndpoints'
import { formatCurrency } from '../utils/format'
import StatCards from '../mocks/statcards.json'

export function useMappedObjects() {
  const [data, setData] = useState({
    users: [],
    categories: [],
    dishes: [],
    coupons: [],
    orders: [],
    paymentMethods: [],
    restaurants: [],
    addresses: [],
    roles: [],
    statCards: StatCards.map((statCard) => ({
      id: statCard.id,
      icon: statCard.icon,
      site: statCard.site,
      title: statCard.title,
      value: statCard.value,
    })),
  })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const fetchPublicData = useCallback(async () => {
    try {
      const [categories, dishes, paymentMethods, restaurants, roles] = await Promise.allSettled([
        GetCategories(),
        GetDishes(),
        GetPaymentMethods(),
        GetRestaurants(),
        GetRoles(),
      ])

      setData((prev) => ({
        ...prev,
        categories: categories.status === 'fulfilled' && categories.value
          ? categories.value.map((element) => ({
              id: element.id,
              name: element.name,
              icon: element.icon,
              slug: element.slug,
            }))
          : [],
        dishes: dishes.status === 'fulfilled' && dishes.value
          ? dishes.value.map((dishe) => ({
              id: dishe.id,
              img: dishe.img,
              category: dishe.category,
              description: dishe.description,
              price: formatCurrency(dishe.price),
              name: dishe.name,
            }))
          : [],
        paymentMethods: paymentMethods.status === 'fulfilled' && paymentMethods.value
          ? paymentMethods.value.map((method) => ({
              id: method.id,
              name: method.name,
              type: method.type,
              description: method.description,
            }))
          : [],
        restaurants: restaurants.status === 'fulfilled' && restaurants.value
          ? restaurants.value.map((restaurant) => ({
              id: restaurant.id,
              tradeName: restaurant.tradeName,
              address: restaurant.address,
              categoryId: restaurant.categoryId,
              openingTime: restaurant.openingTime,
              closingTime: restaurant.closingTime,
              img: restaurant.img,
              rating: restaurant.rating,
              isOpen: restaurant.isOpen,
              deliveryFee: formatCurrency(restaurant.deliveryFee),
              deliveryTime: restaurant.deliveryTime,
              latitude: restaurant.latitude,
              longitude: restaurant.longitude,
              minOrderAmount: restaurant.minOrderAmount,
            }))
          : [],
        roles: roles.status === 'fulfilled' && roles.value
          ? roles.value.map((role) => ({
              id: role.id,
              name: role.name,
              subtitle: role.subtitle,
              site: role.site,
              icon: role.icon,
            }))
          : [],
      }))
    } catch (err) {
      console.error('Error fetching public data:', err)
    }
  }, [])

  const fetchProtectedData = useCallback(async () => {
    try {
      const [users, coupons, orders, addresses] = await Promise.allSettled([
        GetUsers(),
        GetCoupons(),
        GetOrders(),
        GetUserAddresses(1), // fallback, se usa solo si hay usuario autenticado
      ])

      setData((prev) => ({
        ...prev,
        users: users.status === 'fulfilled' && users.value
          ? users.value.map((user) => ({
              id: user.id,
              createdAt: user.createdAt,
              updatedAt: user.updatedAt,
              name: user.name,
              email: user.email,
              phone: user.phone,
              img: user.img,
              roleId: user.roleId,
              lastLoginAt: user.lastLoginAt,
              isActive: user.isActive,
              emailVerifiedAt: user.emailVerifiedAt,
            }))
          : [],
        coupons: coupons.status === 'fulfilled' && coupons.value
          ? coupons.value.map((coupon) => ({
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
          : [],
        orders: orders.status === 'fulfilled' && orders.value
          ? orders.value.map((order) => ({
              id: order.id,
              createdAt: order.createdAt,
              updatedAt: order.updatedAt,
              status: order.status,
              customerId: order.customerId,
              paymentMethodId: order.paymentMethodId,
              addressId: order.addressId,
              subtotal: order.subtotal,
              tax: order.tax,
              deliveryFee: order.deliveryFee,
              total: order.total,
              notes: order.notes,
              cancellationReason: order.cancellationReason,
              restaurantId: order.restaurantId,
              items: order.items,
            }))
          : [],
        addresses: addresses.status === 'fulfilled' && addresses.value
          ? addresses.value.map((ubication) => ({
              id: ubication.id,
              name: ubication.name,
            }))
          : [],
      }))
    } catch (err) {
      console.error('Error fetching protected data:', err)
    }
  }, [])

  useEffect(() => {
    let mounted = true

    const loadData = async () => {
      setLoading(true)
      setError(null)

      // Siempre cargar datos públicos
      await fetchPublicData()

      // Solo cargar datos protegidos si hay token
      const token = localStorage.getItem('authToken')
      if (token) {
        await fetchProtectedData()
      }

      if (mounted) {
        setLoading(false)
      }
    }

    loadData()

    return () => {
      mounted = false
    }
  }, [fetchPublicData, fetchProtectedData])

  return {
    ...data,
    loading,
    error,
    refetch: () => {
      fetchPublicData()
      const token = localStorage.getItem('authToken')
      if (token) fetchProtectedData()
    },
  }
}
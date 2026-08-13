import { API_BASE_URL } from './apiConfig'

const ORDERS_API = `${API_BASE_URL}/orders`
const CREATE_ORDER_ENDPOINT = ORDERS_API
const GET_ORDERS_ENDPOINT = ORDERS_API
const GET_ORDER_BY_ID_ENDPOINT = (id) => `${ORDERS_API}/${id}`
const GET_ORDERS_BY_USER_ENDPOINT = (userId) => `${ORDERS_API}/user/${userId}`
const GET_ORDERS_BY_RESTAURANT_ENDPOINT = (restaurantId) =>
  `${ORDERS_API}/restaurant/${restaurantId}`
const UPDATE_ORDER_ENDPOINT = (id) => `${ORDERS_API}/${id}`

export async function CreateOrder({ order }) {
  try {
    const res = await fetch(CREATE_ORDER_ENDPOINT, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(order),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error creating order', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetOrders() {
  try {
    const res = await fetch(GET_ORDERS_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching orders', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetOrderById({ id }) {
  try {
    const res = await fetch(GET_ORDER_BY_ID_ENDPOINT(id))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching order by id', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetOrdersByUser({ userId }) {
  try {
    const res = await fetch(GET_ORDERS_BY_USER_ENDPOINT(userId))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching orders by user', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetOrdersByRestaurant({ restaurantId }) {
  try {
    const res = await fetch(GET_ORDERS_BY_RESTAURANT_ENDPOINT(restaurantId))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching orders by restaurant', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function UpdateOrder({ id, order }) {
  try {
    const res = await fetch(UPDATE_ORDER_ENDPOINT(id), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(order),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error updating order', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

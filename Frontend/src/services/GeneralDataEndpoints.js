import { API_BASE_URL } from './apiConfig'
import { authHeaders } from './authHelper'

const GET_CATEGORIES_ENDPOINT = `${API_BASE_URL}/categories`
const GET_PAYMENT_METHODS_ENDPOINT = `${API_BASE_URL}/payment-methods`
const GET_ROLES_ENDPOINT = `${API_BASE_URL}/roles`

export async function GetCategories() {
  try {
    const res = await fetch(GET_CATEGORIES_ENDPOINT, {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching categories', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetPaymentMethods() {
  try {
    const res = await fetch(GET_PAYMENT_METHODS_ENDPOINT, {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching payment methods', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetRoles() {
  try {
    const res = await fetch(GET_ROLES_ENDPOINT, {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching roles', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

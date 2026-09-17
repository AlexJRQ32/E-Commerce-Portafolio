import { API_BASE_URL } from './apiConfig'
import { setToken } from './authHelper'

const AUTH_API = `${API_BASE_URL}/auth`
const LOGIN_ENDPOINT = `${AUTH_API}/login`
const REGISTER_ENDPOINT = `${AUTH_API}/register`
const REGISTER_RESTAURANT_ENDPOINT = `${AUTH_API}/register-restaurant`

export async function Login({ user }) {
  try {
    const res = await fetch(LOGIN_ENDPOINT, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(user),
    })

    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error during login', errorText)
      return null
    }

    const data = await res.json()

    if (data?.Token) {
      setToken(data.Token)
    }

    return data
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function Register({ user }) {
  try {
    const res = await fetch(REGISTER_ENDPOINT, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(user),
    })

    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error during registration', errorText)
      return null
    }

    const data = await res.json()

    if (data?.Token) {
      setToken(data.Token)
    }

    return data
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function RegisterRestaurant({ restaurant, token }) {
  try {
    const res = await fetch(REGISTER_RESTAURANT_ENDPOINT, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify(restaurant),
    })

    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error registering restaurant', errorText)
      return { ok: false, message: 'Error registrando restaurante' }
    }

    return { ok: true, data: await res.json() }
  } catch (error) {
    console.error('Error syncing with backend', error)
    return { ok: false, message: 'Error de conexión' }
  }
}

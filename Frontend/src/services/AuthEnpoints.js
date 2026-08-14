import { API_BASE_URL } from './apiConfig'
import { setToken } from './authHelper'

const AUTH_API = `${API_BASE_URL}/auth`
const LOGIN_ENDPOINT = `${AUTH_API}/login`
const REGISTER_ENDPOINT = `${AUTH_API}/register`

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

    // Backend returns { message, token, user } — save token
    if (data?.token) {
      setToken(data.token)
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

    // If backend returns a token on registration, save it
    if (data?.token) {
      setToken(data.token)
    }

    // NOTE: When registering an email that already exists, the backend
    // returns 200 with a neutral message ("Solicitud procesada") for
    // anti-enumeration. No user is created in that case. The caller
    // should check data.message to inform the user appropriately.

    return data
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

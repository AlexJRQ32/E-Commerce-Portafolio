import { API_BASE_URL } from './apiConfig'

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

    return await res.json()
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

    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

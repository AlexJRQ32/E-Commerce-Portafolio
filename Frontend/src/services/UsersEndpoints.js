import { API_BASE_URL } from './apiConfig'
import { authHeaders } from './authHelper'

const USERS_API = `${API_BASE_URL}/users`
const GET_USERS_ENDPOINT = USERS_API
const GET_USER_BY_ID_ENDPOINT = (id) => `${USERS_API}/${id}`
const CREATE_USER_ENDPOINT = USERS_API
const UPDATE_USER_ENDPOINT = (id) => `${USERS_API}/${id}`
const DELETE_USER_ENDPOINT = (id) => `${USERS_API}/${id}`
const GET_USER_ADDRESSES_ENDPOINT = (userId) =>
  `${USERS_API}/${userId}/addresses`

export async function GetUsers() {
  try {
    const res = await fetch(GET_USERS_ENDPOINT, {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching users', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetUserById({ id }) {
  try {
    const res = await fetch(GET_USER_BY_ID_ENDPOINT(id), {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching user by id', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function CreateUser({ user }) {
  try {
    const res = await fetch(CREATE_USER_ENDPOINT, {
      method: 'POST',
      headers: authHeaders(),
      body: JSON.stringify(user),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error creating user', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function UpdateUser({ id, user }) {
  try {
    const res = await fetch(UPDATE_USER_ENDPOINT(id), {
      method: 'PUT',
      headers: authHeaders(),
      body: JSON.stringify(user),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error updating user', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function DeleteUser({ id }) {
  try {
    const res = await fetch(DELETE_USER_ENDPOINT(id), {
      method: 'DELETE',
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error deleting user', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetUserAddresses({ userId }) {
  try {
    const res = await fetch(GET_USER_ADDRESSES_ENDPOINT(userId), {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching user addresses', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

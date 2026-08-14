import { API_BASE_URL } from './apiConfig'
import { authHeaders } from './authHelper'

const DISHES_API = `${API_BASE_URL}/dishes`
const GET_DISHES_ENDPOINT = DISHES_API
const GET_DISH_BY_ID_ENDPOINT = (id) => `${DISHES_API}/${id}`
const CREATE_DISH_ENDPOINT = DISHES_API
const UPDATE_DISH_ENDPOINT = (id) => `${DISHES_API}/${id}`
const DELETE_DISH_ENDPOINT = (id) => `${DISHES_API}/${id}`

export async function GetDishes() {
  try {
    const res = await fetch(GET_DISHES_ENDPOINT, {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching dishes', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function GetDishById({ id }) {
  try {
    const res = await fetch(GET_DISH_BY_ID_ENDPOINT(id), {
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching dish by id', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function CreateDish({ dish }) {
  try {
    const res = await fetch(CREATE_DISH_ENDPOINT, {
      method: 'POST',
      headers: authHeaders(),
      body: JSON.stringify(dish),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error creating dish', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function UpdateDish({ id, dish }) {
  try {
    const res = await fetch(UPDATE_DISH_ENDPOINT(id), {
      method: 'PUT',
      headers: authHeaders(),
      body: JSON.stringify(dish),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error updating dish', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

export async function DeleteDish({ id }) {
  try {
    const res = await fetch(DELETE_DISH_ENDPOINT(id), {
      method: 'DELETE',
      headers: authHeaders(),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error deleting dish', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with backend', error)
    return null
  }
}

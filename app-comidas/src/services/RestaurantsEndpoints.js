const RESTAURANTS_API = 'https://www.apirest-comidas.somee.com/restaurants'
const GET_RESTAURANTS_ENDPOINT = RESTAURANTS_API
const GET_RESTAURANT_BY_ID_ENDPOINT = (id) => `${RESTAURANTS_API}/${id}`
const CREATE_RESTAURANT_ENDPOINT = RESTAURANTS_API
const UPDATE_RESTAURANT_ENDPOINT = (id) => `${RESTAURANTS_API}/${id}`
const DELETE_RESTAURANT_ENDPOINT = (id) => `${RESTAURANTS_API}/${id}`
const GET_RESTAURANT_MENU_ENDPOINT = (id) => `${RESTAURANTS_API}/${id}/menu`
const GET_RESTAURANTS_BY_USER_ENDPOINT = (userId) =>
  `${RESTAURANTS_API}/user/${userId}`

export async function GetRestaurants() {
  try {
    const res = await fetch(GET_RESTAURANTS_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching restaurants', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetRestaurantById({ id }) {
  try {
    const res = await fetch(GET_RESTAURANT_BY_ID_ENDPOINT(id))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching restaurant by id', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function CreateRestaurant({ restaurant }) {
  try {
    const res = await fetch(CREATE_RESTAURANT_ENDPOINT, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(restaurant),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error creating restaurant', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function UpdateRestaurant({ id, restaurant }) {
  try {
    const res = await fetch(UPDATE_RESTAURANT_ENDPOINT(id), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(restaurant),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error updating restaurant', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function DeleteRestaurant({ id }) {
  try {
    const res = await fetch(DELETE_RESTAURANT_ENDPOINT(id), {
      method: 'DELETE',
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error deleting restaurant', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetRestaurantMenu({ id }) {
  try {
    const res = await fetch(GET_RESTAURANT_MENU_ENDPOINT(id))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching restaurant menu', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetRestaurantsByUser({ userId }) {
  try {
    const res = await fetch(GET_RESTAURANTS_BY_USER_ENDPOINT(userId))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching restaurants by user', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

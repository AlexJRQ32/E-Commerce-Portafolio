const GENERAL_DATA_API = 'https://www.apirest-comidas.somee.com/api/generaldata'
const GET_CATEGORIES_ENDPOINT = `${GENERAL_DATA_API}/categories`
const GET_PAYMENT_METHODS_ENDPOINT = `${GENERAL_DATA_API}/payment-methods`
const GET_ROLES_ENDPOINT = `${GENERAL_DATA_API}/roles`

export async function GetCategories() {
  try {
    const res = await fetch(GET_CATEGORIES_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching categories', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetPaymentMethods() {
  try {
    const res = await fetch(GET_PAYMENT_METHODS_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching payment methods', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetRoles() {
  try {
    const res = await fetch(GET_ROLES_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching roles', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

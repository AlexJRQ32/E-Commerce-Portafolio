const COUPONS_API = 'https://www.apirest-comidas.somee.com/api/coupons'
const GET_COUPONS_ENDPOINT = COUPONS_API
const GET_COUPON_BY_ID_ENDPOINT = (id) => `${COUPONS_API}/${id}`
const CREATE_COUPON_ENDPOINT = COUPONS_API
const UPDATE_COUPON_ENDPOINT = (id) => `${COUPONS_API}/${id}`
const DELETE_COUPON_ENDPOINT = (id) => `${COUPONS_API}/${id}`
const GET_AVAILABLE_COUPONS_ENDPOINT = `${COUPONS_API}/available`
const GET_RESERVED_COUPONS_ENDPOINT = (userId) =>
  `${COUPONS_API}/reserved/${userId}`
const GET_COUPONS_BY_USER_ENDPOINT = (userId) => `${COUPONS_API}/user/${userId}`
const RESERVE_COUPON_ENDPOINT = ({ couponId, userId }) =>
  `${COUPONS_API}/${couponId}/apartar/${userId}`

export async function GetCoupons() {
  try {
    const res = await fetch(GET_COUPONS_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching coupons', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetCouponById({ id }) {
  try {
    const res = await fetch(GET_COUPON_BY_ID_ENDPOINT(id))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching coupon by id', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function CreateCoupon({ coupon }) {
  try {
    const res = await fetch(CREATE_COUPON_ENDPOINT, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(coupon),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error creating coupon', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function UpdateCoupon({ id, coupon }) {
  try {
    const res = await fetch(UPDATE_COUPON_ENDPOINT(id), {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(coupon),
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error updating coupon', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function DeleteCoupon({ id }) {
  try {
    const res = await fetch(DELETE_COUPON_ENDPOINT(id), {
      method: 'DELETE',
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error deleting coupon', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetAvailableCoupons() {
  try {
    const res = await fetch(GET_AVAILABLE_COUPONS_ENDPOINT)
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching available coupons', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetReservedCoupons({ userId }) {
  try {
    const res = await fetch(GET_RESERVED_COUPONS_ENDPOINT(userId))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching reserved coupons', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function GetCouponsByUser({ userId }) {
  try {
    const res = await fetch(GET_COUPONS_BY_USER_ENDPOINT(userId))
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error fetching coupons by user', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

export async function ReserveCoupon({ couponId, userId }) {
  try {
    const res = await fetch(RESERVE_COUPON_ENDPOINT({ couponId, userId }), {
      method: 'POST',
    })
    if (!res.ok) {
      const errorText = await res.text()
      console.error('Error reserving coupon', errorText)
      return null
    }
    return await res.json()
  } catch (error) {
    console.error('Error syncing with Somee', error)
    return null
  }
}

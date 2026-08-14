#Auth
• POST https://www.apirest-comidas.somee.com/api/auth/login
• POST https://www.apirest-comidas.somee.com/api/auth/register

---

#Usuarios
• GET https://www.apirest-comidas.somee.com/api/users
• GET https://www.apirest-comidas.somee.com/api/users/{id}
• POST https://www.apirest-comidas.somee.com/api/users
• PUT https://www.apirest-comidas.somee.com/api/users/{id}
• DELETE https://www.apirest-comidas.somee.com/api/users/{id}
• GET https://www.apirest-comidas.somee.com/api/users/{userId}/addresses

---

#Restaurantes
• GET https://www.apirest-comidas.somee.com/api/restaurants
• GET https://www.apirest-comidas.somee.com/api/restaurants/{id}
• POST https://www.apirest-comidas.somee.com/api/restaurants
• PUT https://www.apirest-comidas.somee.com/api/restaurants/{id}
• DELETE https://www.apirest-comidas.somee.com/api/restaurants/{id}
• GET https://www.apirest-comidas.somee.com/api/restaurants/{id}/menu
• GET https://www.apirest-comidas.somee.com/api/restaurants/user/{userId}

---

#Pedidos
• POST https://www.apirest-comidas.somee.com/api/orders
• GET https://www.apirest-comidas.somee.com/api/orders
• GET https://www.apirest-comidas.somee.com/api/orders/{id}
• GET https://www.apirest-comidas.somee.com/api/orders/user/{userId}
• GET https://www.apirest-comidas.somee.com/api/orders/restaurant/{restaurantId}
• PUT https://www.apirest-comidas.somee.com/api/orders/{id}

---

#Platillos
• GET https://www.apirest-comidas.somee.com/api/dishes
• GET https://www.apirest-comidas.somee.com/api/dishes/{id}
• POST https://www.apirest-comidas.somee.com/api/dishes
• PUT https://www.apirest-comidas.somee.com/api/dishes/{id}
• DELETE https://www.apirest-comidas.somee.com/api/dishes/{id}

---

#Cupones
• GET https://www.apirest-comidas.somee.com/api/coupons
• GET https://www.apirest-comidas.somee.com/api/coupons/{id}
• POST https://www.apirest-comidas.somee.com/api/coupons
• PUT https://www.apirest-comidas.somee.com/api/coupons/{id}
• DELETE https://www.apirest-comidas.somee.com/api/coupons/{id}
• GET https://www.apirest-comidas.somee.com/api/coupons/available
• GET https://www.apirest-comidas.somee.com/api/coupons/reserved/{userId}
• GET https://www.apirest-comidas.somee.com/api/coupons/user/{userId}
• POST https://www.apirest-comidas.somee.com/api/coupons/{couponId}/apartar/{userId}

---

#Datos Generales
• GET https://www.apirest-comidas.somee.com/api/generaldata/categories
• GET https://www.apirest-comidas.somee.com/api/generaldata/payment-methods
• GET https://www.apirest-comidas.somee.com/api/generaldata/roles

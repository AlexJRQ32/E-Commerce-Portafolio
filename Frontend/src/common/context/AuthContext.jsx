import { createContext, useState, useContext, useEffect } from 'react'
import { getToken, setToken, clearToken, authHeaders } from '../services/authHelper'
import { Login, Register } from '../services/AuthEnpoints'

export const AuthContext = createContext()

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  const login = async (email, password) => {
    const res = await Login({ user: { email, password } })
    if (res?.Token) {
      setToken(res.Token)
      setUser({
        id: res.User.Id,
        name: res.User.Name,
        email: res.User.Email,
        phone: res.User.Phone,
        img: res.User.Img,
        role: res.User.Role
      })
      return { success: true }
    }
    return { success: false, message: res?.Message || 'Credenciales incorrectas' }
  }

  const register = async (data) => {
    const res = await Register({ user: data })
    if (res?.UserId && res.UserId > 0) {
      return { success: true, userId: res.UserId, roleId: res.RoleId }
    }
    if (res?.UserId === 0) {
      return { success: false, message: 'El email ya está registrado', exists: true }
    }
    return { success: false, message: res?.Message || 'Error en el registro' }
  }

  const logout = () => {
    clearToken()
    setUser(null)
  }

  const checkAuth = async () => {
    const token = getToken()
    if (token) {
      // Optionally validate token with backend /me endpoint
      // For now, assume valid if present
      try {
        // Could call /api/users/me if backend has it
      } catch {
        logout()
      }
    }
    setLoading(false)
  }

  useEffect(() => {
    checkAuth()
  }, [])

  return (
    <AuthContext.Provider value={{ user, login, register, logout, loading }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
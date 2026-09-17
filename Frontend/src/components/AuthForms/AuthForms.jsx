import { useState } from 'react'
import { NavLink, useNavigate } from 'react-router-dom'
import { ButtonRedirect } from '../Button/Button'
import './AuthForms.css'
import { RoleCard } from '../Cards/Cards'
import { useMappedObjects } from '../../common/hooks/useMappedObjects'
import { AuthInput, AuthSelect } from './AuthInput'
import { useAuth } from '../../common/context/AuthContext'
import { RegisterRestaurant } from '../../common/services/AuthEnpoints'

export function SignInForm() {
  const navigate = useNavigate()
  const { login } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    const res = await login(email, password)
    setLoading(false)
    if (res.success) {
      navigate('/home')
    } else {
      setError(res.message)
    }
  }

  return (
    <>
      <form className='auth-form' onSubmit={handleSubmit}>
        <h1>Sign In</h1>
        {error && <div className="auth-error">{error}</div>}
        <AuthInput
          name={'email'}
          type={'text'}
          title={'Email'}
          icon={'envelope'}
          placeholder={'example@gmail.com'}
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <AuthInput
          name={'password'}
          type={'password'}
          title={'Password'}
          icon={'lock'}
          placeholder={'••••••••'}
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <div className="auth-input">
          <NavLink to={"#"}><p>Forgot your password?</p></NavLink>
        </div>
        <ButtonRedirect
          className={'action-large'}
          type={'submit'}
          title={loading ? 'Signing in...' : 'SIGN IN'}
        />
        <NavLink to={'/auth/sign-up'}><p>Don't have an account? Sign up</p></NavLink>
      </form>
    </>
  )
}

export function SignUpForm() {
  const navigate = useNavigate()
  const { register } = useAuth()
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    countryCode: '+506',
    roleId: 3
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const COUNTRY_CODES = [
    { id: 1, code: "+1", country: "US", label: "+1 (US)" },
    { id: 2, code: "+34", country: "ES", label: "+34 (ES)" },
    { id: 3, code: "+52", country: "MX", label: "+52 (MX)" },
    { id: 4, code: "+54", country: "AR", label: "+54 (AR)" },
    { id: 5, code: "+506", country: "CR", label: "+506 (CR)" }
  ];

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    if (formData.password !== formData.confirmPassword) {
      setError('Las contraseñas no coinciden')
      return
    }
    setLoading(true)
    const res = await register({
      Name: formData.name,
      Email: formData.email,
      Password: formData.password,
      Phone: `${formData.countryCode} ${formData.phone}`,
      RoleId: formData.roleId
    })
    setLoading(false)
    if (res.success) {
      if (res.roleId === 2) {
        navigate('/auth/register-business')
      } else {
        navigate('/auth/sign-in')
      }
    } else {
      setError(res.message)
    }
  }

  return (
    <>
      <form className='auth-form' onSubmit={handleSubmit}>
        <h1>Sign Up</h1>
        {error && <div className="auth-error">{error}</div>}
        <AuthInput
          name={'name'}
          type={'text'}
          title={'Name'}
          icon={'user'}
          placeholder={'Enter your full name'}
          value={formData.name}
          onChange={handleChange}
          required
        />
        <AuthInput
          name={'email'}
          type={'text'}
          title={'Email'}
          icon={'envelope'}
          placeholder={'example@gmail.com'}
          value={formData.email}
          onChange={handleChange}
          required
        />
        <AuthInput
          name={'password'}
          type={'password'}
          title={'Password'}
          icon={'key'}
          placeholder={'••••••••'}
          value={formData.password}
          onChange={handleChange}
          required
        />
        <AuthInput
          name={'confirmPassword'}
          type={'password'}
          title={'Confirm Password'}
          icon={'lock'}
          placeholder={'••••••••'}
          value={formData.confirmPassword}
          onChange={handleChange}
          required
        />
        <div className="phone-container">
          <AuthSelect
            name={'countryCode'}
            title={'Country Code'}
            icon={'globe'}
            objects={COUNTRY_CODES}
            value={formData.countryCode}
            onChange={handleChange}
          />
          <AuthInput
            name={'phone'}
            type={'tel'}
            title={'Phone Number'}
            icon={'phone'}
            placeholder={'123-456-789'}
            value={formData.phone}
            onChange={handleChange}
            required
          />
        </div>
        <div className="role-selection">
          <label>
            <input
              type="radio"
              name="roleId"
              value={3}
              checked={formData.roleId === 3}
              onChange={handleChange}
            />
            <span>Customer (Order food)</span>
          </label>
          <label>
            <input
              type="radio"
              name="roleId"
              value={2}
              checked={formData.roleId === 2}
              onChange={handleChange}
            />
            <span>Business (Register restaurant)</span>
          </label>
        </div>
        <ButtonRedirect
          className={'action-large'}
          type={'submit'}
          title={loading ? 'Creating account...' : 'SIGN UP'}
        />
        <NavLink to={'/auth/sign-in'}><p>Already have an account? Sign in</p></NavLink>
      </form>
    </>
  )
}

export function ChooseRoleForm() {
  const { roles } = useMappedObjects()
  return(
    <div className="role-container">
      <span><h2>Rappi</h2><h2>'Doz</h2></span>
      <h1>Who are you?</h1>
      <RoleCard roles={roles} />
      <NavLink to={'/auth/sign-in'}>
        <i className='fas fa-arrow-left'></i>
        Go Back
      </NavLink>
    </div>
  )
}

export function RegisterBusinessForm() {
  const navigate = useNavigate()
  const { categories } = useMappedObjects()
  const { logout } = useAuth()
  const [formData, setFormData] = useState({
    tradeName: '',
    address: '',
    openingTime: '',
    closingTime: '',
    categoryId: '',
    deliveryFee: '',
    deliveryTime: '',
    latitude: '',
    longitude: '',
    minOrderAmount: ''
  })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const token = localStorage.getItem('authToken')
      const res = await RegisterRestaurant({
        TradeName: formData.tradeName,
        CategoryId: parseInt(formData.categoryId),
        Address: formData.address,
        OpeningTime: formData.openingTime,
        ClosingTime: formData.closingTime,
        DeliveryFee: parseFloat(formData.deliveryFee),
        DeliveryTime: formData.deliveryTime,
        Latitude: formData.latitude ? parseFloat(formData.latitude) : null,
        Longitude: formData.longitude ? parseFloat(formData.longitude) : null,
        MinOrderAmount: parseFloat(formData.minOrderAmount) || 0
      }, token)
      setLoading(false)
      if (res.ok) {
        navigate('/dashboard')
      } else {
        setError(res.message || 'Error registrando restaurante')
      }
    } catch {
      setLoading(false)
      setError('Error de conexión')
    }
  }

  return(
    <>
      <form className='auth-form' onSubmit={handleSubmit}>
        <div className="form-header">
          <span><h2>Rappi</h2><h2>'Doz</h2></span>
          <h1>New Business</h1>
        </div>
        {error && <div className="auth-error">{error}</div>}
        <AuthInput
          name={'tradeName'}
          type={'text'}
          title={'Business Name'}
          placeholder={'Ready Pizza'}
          icon={'store'}
          value={formData.tradeName}
          onChange={handleChange}
          required
        />
        <AuthInput
          name={'address'}
          type={'text'}
          title={'Exact Address'}
          placeholder={'Tibas, Colima'}
          icon={'location-dot'}
          value={formData.address}
          onChange={handleChange}
          required
        />
        <div className="schedule-container">
          <AuthInput
            name={'openingTime'}
            type={'time'}
            title={'Opening'}
            icon={'clock'}
            value={formData.openingTime}
            onChange={handleChange}
            required
          />
          <AuthInput
            name={'closingTime'}
            type={'time'}
            title={'Closing'}
            icon={'moon'}
            value={formData.closingTime}
            onChange={handleChange}
            required
          />
        </div>
        <AuthSelect
          name={'categoryId'}
          title={'Select Category'}
          icon={'tag'}
          objects={categories}
          value={formData.categoryId}
          onChange={handleChange}
          required
        />
        <AuthInput
          name={'deliveryFee'}
          type={'number'}
          title={'Delivery Fee'}
          placeholder={'1000'}
          icon={'dollar-sign'}
          value={formData.deliveryFee}
          onChange={handleChange}
          required
        />
        <AuthInput
          name={'deliveryTime'}
          type={'text'}
          title={'Delivery Time'}
          placeholder={'30 min'}
          icon={'clock'}
          value={formData.deliveryTime}
          onChange={handleChange}
          required
        />
        <div className="location-fields">
          <AuthInput
            name={'latitude'}
            type={'number'}
            step="any"
            title={'Latitude'}
            placeholder={'9.9620'}
            icon={'map-marker-alt'}
            value={formData.latitude}
            onChange={handleChange}
          />
          <AuthInput
            name={'longitude'}
            type={'number'}
            step="any"
            title={'Longitude'}
            placeholder={'-84.0925'}
            icon={'map-marker-alt'}
            value={formData.longitude}
            onChange={handleChange}
          />
        </div>
        <AuthInput
          name={'minOrderAmount'}
          type={'number'}
          step="0.01"
          title={'Min Order Amount'}
          placeholder={'0'}
          icon={'dollar-sign'}
          value={formData.minOrderAmount}
          onChange={handleChange}
        />
        <ButtonRedirect
          className={'action-large'}
          type={'submit'}
          title={loading ? 'Registering...' : 'REGISTER BUSINESS'}
        />
        <NavLink to={'/auth/sign-in'} onClick={logout}>
          <i className='fas fa-arrow-left'></i>
          Cancel registration
        </NavLink>
      </form>
    </>
  )
}

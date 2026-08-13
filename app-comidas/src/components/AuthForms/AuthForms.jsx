import { NavLink } from 'react-router-dom'
import { ButtonRedirect } from '../Button/Button'
import './AuthForms.css'
import { RoleCard } from '../Cards/Cards'
import { useMappedObjects } from '../../hooks/useMappedObjects'
import { AuthInput, AuthSelect } from './AuthInput'

export function SignInForm() {
  return (
    <>
      <form className='auth-form'>
        <h1>Sign In</h1>
        <AuthInput name={'email'} type={'text'} title={'Email'} icon={'envelope'} placeholder={'example@gmail.com'}/>
        <AuthInput name={'password'} type={'password'} title={'Password'} icon={'lock'} placeholder={'••••••••'}/>
        <div className="auth-input">
          <NavLink to={"#"}><p>Forgot your password?</p></NavLink>
        </div>
        <ButtonRedirect className={'action-large'} site={'/'} title={'SIGN IN'}/>
        <NavLink to={'/auth/sign-up'}><p>Don't have an account? Sign up</p></NavLink>
      </form>
    </>
  )
}

export function SignUpForm() {
  const COUNTRY_CODES = [
    { id: 1, code: "+1", country: "US", label: "+1 (US)" },
    { id: 2, code: "+34", country: "ES", label: "+34 (ES)" },
    { id: 3, code: "+52", country: "MX", label: "+52 (MX)" },
    { id: 4, code: "+54", country: "AR", label: "+54 (AR)" },
    { id: 5, code: "+506", country: "CR", label: "+506 (CR)" }
  ];

  return (
    <>
      <form className='auth-form'>
        <h1>Sign Up</h1>
        <AuthInput name={'name'} type={'text'} title={'Name'} icon={'user'} placeholder={'Enter your full name'}/>
        <AuthInput name={'email'} type={'text'} title={'Email'} icon={'envelope'} placeholder={'example@gmail.com'}/>
        <AuthInput name={'password'} type={'password'} title={'Password'} icon={'key'} placeholder={'••••••••'}/>
        <AuthInput name={'confirm-password'} type={'password'} title={'Confirm Password'} icon={'lock'} placeholder={'••••••••'}/>
        <div class="phone-container">
          <AuthSelect name={'country-code'} title={'Country Code'} icon={'globe'} objects={COUNTRY_CODES}/>
          <AuthInput name={'phone'} type={'tel'} title={'Phone Number'} icon={'phone'} placeholder={'123-456-789'}/>
        </div>
        <ButtonRedirect className={'action-large'} site={'/auth/choose-role'} title={'SIGN UP'}/>
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
  const { categories } = useMappedObjects()

  return(
    <>
      <form className='auth-form'>
        <div className="form-header">
          <span><h2>Rappi</h2><h2>'Doz</h2></span>
          <h1>New Business</h1>
        </div>
        <AuthInput name={'business-name'} type={'text'} title={'Business Name'} placeholder={'Ready Pizza'} icon={'store'}/>
        <AuthInput name={'address'} type={'text'} title={'Exact Address'} placeholder={'Tibas, Colima'} icon={'location-dot'}/>
        <div className="schedule-container">
          <AuthInput name={'open-time'} type={'time'} title={'Opening'} icon={'clock'}/>
          <AuthInput name={'closed-time'} type={'time'} title={'Closing'} icon={'moon'}/>
        </div>
        <AuthSelect name={'categories'} title={'Select Category'} icon={'tag'} objects={categories} dataObject={categories} />
        <ButtonRedirect className={'action-large'} site={'/'} title={'REGISTER BUSINESS'}/>
        <NavLink to={'/auth/sign-in'}>
          <i className='fas fa-arrow-left'></i>
          Cancel registration
        </NavLink>
      </form>
    </>
  )
}

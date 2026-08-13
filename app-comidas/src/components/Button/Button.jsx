import './Button.css'
import { useNavigate } from 'react-router-dom'

export function ButtonRedirect({ className, icon, title, type, site }) {
  const navigate = useNavigate()

  const handleClick = () => navigate(`/${site}`)
  return (
    <button
      className={`btn-redirect btn-${className}`}
      onClick={handleClick}
      type={type}
    >
      <i className={`fa-solid fa-${icon}`}></i>
      {title}
    </button>
  )
}

export function ButtonAction({ className, icon, type, onclick}) {
  return (
    <button
      className={`btn-action btn-${className}`}
      onClick={onclick}
      type={type}
    >
      <i className={`fa-solid fa-${icon}`}></i>
    </button>
  )
}

export function ButtonActionLarge({ title, type, onclick }) {
  return (
    <button
      className='btn-action-large'
      onClick={onclick}
      type={type}
    >
      {title}
    </button>
  )
}
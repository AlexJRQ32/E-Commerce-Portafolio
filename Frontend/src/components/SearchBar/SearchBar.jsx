import { ButtonRedirect } from '../Button/Button'
import './SearchBar.css'

export function SearchBar({ icon, className }) {
  return (
    <>
      <form className={`search-bar search-bar-${className}`}>
        <span>
          <i className={`fa-solid fa-${icon}`}></i>
          <input type="text" placeholder="Search what you want to eat" />
        </span>
        <ButtonRedirect title={'Search'} site={'search'} />
      </form>
    </>
  )
}

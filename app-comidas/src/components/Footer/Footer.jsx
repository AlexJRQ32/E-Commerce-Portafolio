import { ButtonRedirect } from "../Button/Button";
import './Footer.css'

export function Footer() {
  return(
    <div className="footer">
      <footer>
        <h1>Ready to Order?</h1>
        <ButtonRedirect title={'Explore restaurants'} site={'search'}/>
      </footer>
    </div>
  )
}
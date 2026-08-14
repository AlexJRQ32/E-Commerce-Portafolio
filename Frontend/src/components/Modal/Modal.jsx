import './Modal.css'
import { ButtonAction } from '../Button/Button'

export function Modal({ isOpen, onClose, children, subtitle, title, form, isCoupon }) {
  if(!isOpen) return

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-card" onClick={(e) => e.stopPropagation()}>
        <div className="modal-layout">
          <div className="modal-content">
            <div className="modal-header">
              <div className="titles">
                <h4>{subtitle}</h4>
                <h2>{title}</h2>
              </div>
              <div className="button">
                <ButtonAction
                  className={'close'}
                  icon={'xmark'}
                  onclick={onClose}
                />
              </div>
            </div>
            <div className="modal-main">
              <div className={isCoupon ? 'modal-hidden' : 'modal-main-left'}>
                <div className={isCoupon ? "modal-hidden" : "circle-image"}>
                  <img
                    src={children && children[0]?.img ? children[0].img : ''}
                    alt={children && children[0]?.name ? children[0].name : 'User'}
                  />
                  <span className={isCoupon ? "modal-hidden" : "edit"}>
                    <ButtonAction className={isCoupon ? "modal-hidden" : 'edit'} icon={'pencil'} />
                  </span>
                </div>
                <p className={isCoupon ? "modal-hidden" : "text-muted"}>Change image</p>
              </div>
              <div className={isCoupon ? "modal-main-large" : "modal-main-right"}>{form}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
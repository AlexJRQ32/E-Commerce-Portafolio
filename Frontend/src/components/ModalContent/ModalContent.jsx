import './ModalContent.css'
import { ButtonActionLarge } from '../Button/Button'

export function FormUser({ children, onEdit }) {
  return (
    <form className="form-modal">
      <label htmlFor="name">Name</label>
      <div className="input-container">
        <i className="fas fa-user-circle input-icon"></i>
        <input
          type="text"
          id="name"
          name="name"
          className="custom-input"
          defaultValue={onEdit ?? children[0].name}
        />
      </div>

      <label htmlFor="email">Email</label>
      <div className="input-container">
        <i className="fas fa-envelope input-icon"></i>
        <input
          type="email"
          id="email"
          name="email"
          className="custom-input"
          defaultValue={onEdit ?? children[0].email}
        />
      </div>

      <label htmlFor="new-password">New Password</label>
      <div className="input-container">
        <i className="fas fa-lock input-icon"></i>
        <input
          type="password"
          id="new-password"
          name="new-password"
          className="custom-input"
          defaultValue={onEdit ?? ''}
        />
      </div>
      <p>* Leave blank to not change</p>

      <ButtonActionLarge
        icon={'floppy-disk'}
        title={'Update Profile'}
        type={'submit'}
      />
    </form>
  )
}

export function FormDish({ children, onEdit  }) {
  return (
    <form className="form-modal">
      <label htmlFor="name">Name</label>
      <div className="input-container">
        <i className="fas fa-utensils input-icon"></i>
        <input
          type="text"
          id="name"
          name="name"
          className="custom-input"
          defaultValue={onEdit ?? children[0].name}
        />
      </div>

      <label htmlFor="category">Category</label>
      <div className="input-container">
        <i className="fas fa-tags input-icon"></i>
        <input
          type="text"
          id="category"
          name="category"
          className="custom-input"
          defaultValue={onEdit ?? children[0].category}
        />
      </div>

      <label htmlFor="price">Price</label>
      <div className="input-container">
        <i className="fas fa-dollar-sign input-icon"></i>
        <input
          type="number"
          id="price"
          name="price"
          className="custom-input"
          defaultValue={onEdit ?? children[0].price}
        />
      </div>

      <ButtonActionLarge
        icon={'floppy-disk'}
        title={'Update Dish'}
        type={'submit'}
      />
    </form>
  )
}

export function FormRestaurant({ children, onEdit }) {
  return (
    <form className="form-modal">
      <label htmlFor="name">Name</label>
      <div className="input-container">
        <i className="fas fa-utensils input-icon"></i>
        <input
          type="text"
          id="name"
          name="name"
          className="custom-input"
          defaultValue={onEdit ?? children[0].name}
        />
      </div>

      <label htmlFor="schedule">Schedule</label>
      <div className="input-container">
        <i className="fas fa-calendar-days input-icon"></i>
        <input
          type="text"
          id="schedule"
          name="schedule"
          className="custom-input"
          defaultValue={onEdit ?? children[0].schedule}
        />
      </div>

      <label htmlFor="location">Location</label>
      <div className="input-container">
        <i className="fas fa-map-marker-alt input-icon"></i>
        <input
          type="text"
          id="location"
          name="location"
          className="custom-input"
          defaultValue={onEdit ?? children[0].location}
        />
      </div>

      <label htmlFor="delivery-fee">Delivery Fee</label>
      <div className="input-container">
        <i className="fas fa-dollar-sign input-icon"></i>
        <input
          type="text"
          id="delivery-fee"
          name="delivery-fee"
          className="custom-input"
          defaultValue={onEdit ?? children[0].deliveryFee}
        />
      </div>

      <label htmlFor="delivery-time">Delivery Time</label>
      <div className="input-container">
        <i className="fas fa-clock input-icon"></i>
        <input
          type="text"
          id="delivery-time"
          name="delivery-time"
          className="custom-input"
          defaultValue={onEdit ?? children[0].deliveryTime}
        />
      </div>

      <ButtonActionLarge
        icon={'floppy-disk'}
        title={'Update Restaurant'}
        type={'submit'}
      />
    </form>
  )
}

export function FormCoupon({ children, onEdit }) {
  return (
    <form className="form-modal">
      <label htmlFor="name">Name</label>
      <div className="input-container">
        <i className="fas fa-ticket input-icon"></i>
        <input
          type="text"
          id="name"
          name="name"
          className="custom-input"
          defaultValue={onEdit ?? children[0].name}
        />
      </div>

      <label htmlFor="discount">Discount</label>
      <div className="input-container">
        <i className="fas fa-percentage input-icon"></i>
        <input
          type="text"
          id="discount"
          name="discount"
          className="custom-input"
          defaultValue={onEdit ?? children[0].discount}
        />
      </div>

      <label htmlFor="short-description">Short Description</label>
      <div className="input-container">
        <i className="fas fa-align-left input-icon"></i>
        <input
          type="text"
          id="short-description"
          name="short-description"
          className="custom-input"
          defaultValue={onEdit ?? children[0].short_description}
        />
      </div>

      <label htmlFor="description">Description</label>
      <div className="input-container">
        <i className="fas fa-file input-icon"></i>
        <input
          type="text"
          id="description"
          name="description"
          className="custom-input"
          defaultValue={onEdit ?? children[0].description}
        />
      </div>

      <label htmlFor="quantity">Quantity</label>
      <div className="input-container">
        <i className="fas fa-boxes input-icon"></i>
        <input
          type="text"
          id="quantity"
          name="quantity"
          className="custom-input"
          defaultValue={onEdit ?? children[0].quantity}
        />
      </div>

      <ButtonActionLarge
        icon={'floppy-disk'}
        title={'Update Coupon'}
        type={'submit'}
      />
    </form>
  )
}
import { ButtonAction } from "../../../components/Button/Button"
import { CrudCard } from "../../../components/Cards/Cards"
import { useMappedObjects } from '../../../hooks/useMappedObjects'
import { FormRestaurant } from '../../../components/ModalContent/ModalContent'
import { Modal } from '../../../components/Modal/Modal'
import { useModal } from '../../../hooks/useModal'

export function RestaurantsDashboard(){
  const { restaurants } = useMappedObjects()
  const { isClose, openModal, isOpen } = useModal()

  return(
    <div className="content-section">
          <Modal
            isOpen={openModal}
            onClose={isClose}
            subtitle={`"Manage your restaurant locations"`}
            title={'Update Restaurant'}
            children={restaurants}
            form={<FormRestaurant children={restaurants} onClose={isClose} />}
          />
          <div className="content-header">
            <h1>Our Locations</h1>
            <ButtonAction icon={'plus'} className={'add'} />
          </div>
          <div className="content-main">
            {restaurants.map((restaurant) => (
              <CrudCard
                key={restaurant.id}
                img={restaurant.img}
                attribute={restaurant.address}
                icon={'location-dot'}
                name={restaurant.tradeName}
                onclick={isOpen}
              />
            ))}
          </div>
        </div>
  )
}
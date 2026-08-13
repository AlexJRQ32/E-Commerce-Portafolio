import { ButtonAction } from '../../../components/Button/Button'
import { CrudCard } from '../../../components/Cards/Cards'
import { useMappedObjects } from '../../../hooks/useMappedObjects'
import { Modal } from '../../../components/Modal/Modal'
import { FormDish } from '../../../components/ModalContent/ModalContent'
import { useModal } from '../../../hooks/useModal'

export function MyMenuDashboard() {
  const { dishes } = useMappedObjects()
  const { isOpen, openModal, isClose } = useModal()

  return (
    <div className="content-section">
      <Modal
        isOpen={openModal}
        onClose={isClose}
        subtitle={`"Your Dishes, your flavor"`}
        title={'Update Dish RappiDoz'}
        children={dishes}
        form={<FormDish children={dishes} onClose={isClose} />}
      />
      <div className="content-header">
        <h1>My Dishes</h1>
        <ButtonAction icon={'plus'} className={'add'} onclick={isOpen} />
      </div>
      <div className="content-main">
        {dishes.map((dishe) => (
          <CrudCard
            key={dishe.id}
            img={dishe.img}
            attribute={dishe.price}
            name={dishe.name}
            onclick={isOpen}
          />
        ))}
      </div>
    </div>
  )
}

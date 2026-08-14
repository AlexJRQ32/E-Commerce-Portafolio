import { ButtonAction } from '../../../../components/Button/Button'
import { CrudCard } from '../../../../components/Cards/Cards'
import { useMappedObjects } from '../../../../common/hooks/useMappedObjects'
import { FormUser } from '../../../../components/ModalContent/ModalContent'
import { Modal } from '../../../../components/Modal/Modal'
import { useModal } from '../../../../common/hooks/useModal'

export function UsersDashboard() {
  const { users } = useMappedObjects()
  const { isClose, openModal, isOpen } = useModal()

  return (
    <div className="content-section">
      <Modal
        isOpen={openModal}
        onClose={isClose}
        subtitle={`"Manage your users"`}
        title={'Update User'}
        children={users}
        form={<FormUser children={users} onClose={isClose} />}
      />
      <div className="content-header">
        <h1>Users</h1>
        <ButtonAction icon={'plus'} className={'add'} onclick={isOpen} />
      </div>
      <div className="content-main">
        {users.map((user) => (
          <CrudCard
            key={user.id}
            img={user.img}
            attribute={user.role}
            icon={'briefcase'}
            name={user.name}
            onclick={isOpen}
          />
        ))}
      </div>
    </div>
  )
}

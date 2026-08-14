import { ButtonAction } from '../../../../components/Button/Button'
import { CouponTicket } from '../../../../components/Coupons/Coupons'
import { useMappedObjects } from '../../../../common/hooks/useMappedObjects'
import './CouponsDashboard.css'
import { FormCoupon } from '../../../../components/ModalContent/ModalContent'
import { Modal } from '../../../../components/Modal/Modal'
import { useModal } from '../../../../common/hooks/useModal'

export function CouponsDashboard() {
  const { coupons } = useMappedObjects()
  const { isClose, openModal, isOpen } = useModal()
  const isCoupon = true

  return (
    <div className="content-section">
      <Modal
        isOpen={openModal}
        onClose={isClose}
        subtitle={`"Manage your coupons"`}
        title={'Update Coupon'}
        children={coupons}
        form={<FormCoupon children={coupons} onClose={isClose} />}
        isCoupon={isCoupon}
      />
      <div className="content-header">
        <h1>Coupons</h1>
        <ButtonAction icon={'plus'} className={'add'} onclick={isOpen} />
      </div>
      <div className="content-main">
        <CouponTicket coupons={coupons} className={'coupon-buttons'} onclick={isOpen} />
      </div>
    </div>
  )
}

import { useContext } from 'react'
import { ModalContext } from '../context/modal.jsx'

export function useModal() {
  const { openModal, setOpenModal, onEdit, setOnEdit } = useContext(ModalContext)

  const isOpen = () => setOpenModal(true)

  const isClose = () => setOpenModal(false)

  return { setOpenModal, isOpen, isClose, openModal, onEdit, setOnEdit }
}

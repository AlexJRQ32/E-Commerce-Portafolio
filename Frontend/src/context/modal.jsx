import { createContext, useState } from 'react'

export const ModalContext = createContext()

export function ModalProvider({ children }) {
    const [openModal, setOpenModal] = useState(false)
    const [onEdit, setOnEdit] = useState(null)
  return (
    <ModalContext.Provider value={{ openModal, setOpenModal, onEdit, setOnEdit }}>
      {children}
    </ModalContext.Provider>
  )
}
import './AuthForms.css'

export function AuthInput({ name, type, title, placeholder, icon }) {
  return(
    <div className="auth-input">
      <label htmlFor={name}>{title}</label>
      <div className="input-wrap">
        <input type={type} name={name} placeholder={placeholder} />
        <i className={`fas fa-${icon}`} ></i>
      </div>
    </div>
  )
}

export function AuthSelect({ name, title, icon, objects, dataObject }) {
  const isCategories = () => {
    if(objects == dataObject) return true
    return false
  } 

  return(
    <div className="auth-input">
      <label htmlFor={name}>{title}</label>
      <div className="input-wrap">
        <select id={name} name={name}>
          {isCategories() 
            ? objects.map(object => (
            <option key={object.id} value={object.name}>{object.name}</option>
          ))
            : objects.map(object => (
            <option key={object.id} value={object.code}>{object.label}</option>
          ))
          }
        </select>
        <i className={`fas fa-${icon}`}></i>
      </div>
    </div>
  )
}
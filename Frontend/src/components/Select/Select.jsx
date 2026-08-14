import './Select.css'

export function Select({ parameters, name, title }) {
  return(
    <select name={name} className='select'>
      <option value="">-- Choose a {title} --</option>
      {
        parameters.map(parameter => (
          <option value={parameter.id}>{parameter.name}</option>
        ))
      }
    </select>
  )
}
import './Search.css'
import { SearchBar } from '../../components/SearchBar/SearchBar'
import { RestaurantLargeCard } from '../../components/Cards/Cards'
import { CategoryCard } from '../../components/Cards/Cards'
import { useMappedObjects } from '../../hooks/useMappedObjects'

export function Search() {
  const { restaurants, categories } = useMappedObjects()
  const restaurantsLength = restaurants.length

  return (
    <div className="page" style={{ backgroundColor: '#1e1c22' }}>
      <section className="search-section">
        <div className="header-search">
          <h1>Your next craving</h1>
          <p>Search by name, food, area or category</p>
          <SearchBar
            icon={'search'}
            className={'in-search'}
          />
        </div>
        <div className="browse-categories">
          <h2>Browse by category</h2>
          <CategoryCard categories={categories} />
        </div>
      </section>
      <section className="show-section">
        <div className="header-show">
          <strong>Restaurants</strong>
          <span>{restaurantsLength} locals</span>
        </div>
        <RestaurantLargeCard restaurants={restaurants} />
      </section>
    </div>
  )
}

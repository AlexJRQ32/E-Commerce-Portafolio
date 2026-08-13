import { ButtonRedirect } from '../../../components/Button/Button'
import { StatCard } from '../../../components/Cards/Cards'
import { useMappedObjects } from '../../../hooks/useMappedObjects'
import './Overview.css'

export function OverviewDashboard() {
  const { statCards } = useMappedObjects()

  return (
    <div className="content-section">
      <div className="overview-header">
        <h1>Welcome, User</h1>
        <p>Real-time administration panel</p>
      </div>
      <div className="content-main">
        <StatCard statcards={statCards} />
        <div className="quick-access">
          <h2>Quick Access</h2>
          <div className="buttons">
            <ButtonRedirect
              title={'View Catalog'}
              site={'/dashboard/my-menu'}
              className={'black'}
            />
            <ButtonRedirect
              title={'Manage Locations'}
              site={'/dashboard/restaurants'}
              className={'clear'}
            />
          </div>
        </div>
      </div>
    </div>
  )
}

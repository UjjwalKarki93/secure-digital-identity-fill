import { NavLink, Navigate, Route, Routes } from 'react-router-dom';
import Scan from './pages/Scan';
import Profile from './pages/Profile';
import { useAuth } from './hooks/useAuth';

function RequireAuth({ children }) {
  const { isAuthenticated } = useAuth();
  if (!isAuthenticated) return <Navigate to="/" replace />;
  return children;
}

export default function App() {
  return (
    <div style={{ paddingBottom: '4rem' }}>
      <header className="app-header">
        <div className="container" style={{ padding: 0 }}>
          <h1>Nagarik</h1>
          <p>National Digital Identity</p>
        </div>
      </header>

      <Routes>
        <Route path="/" element={<Scan />} />
        <Route path="/scan" element={<Navigate to="/" replace />} />
        <Route path="/consent" element={<Navigate to="/" replace />} />
        <Route
          path="/profile"
          element={(
            <RequireAuth>
              <Profile />
            </RequireAuth>
          )}
        />
      </Routes>

      <nav className="nav">
        <NavLink to="/" end>Verify</NavLink>
        <NavLink to="/profile">Profile</NavLink>
      </nav>
    </div>
  );
}

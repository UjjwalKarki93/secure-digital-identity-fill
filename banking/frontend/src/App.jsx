import { NavLink, Route, Routes } from 'react-router-dom';
import BankLogo from './components/BankLogo';
import Home from './pages/Home';
import OpenAccount from './pages/OpenAccount';

export default function App() {
  return (
    <div>
      <header className="site-header">
        <div className="container">
          <NavLink to="/">
            <BankLogo />
          </NavLink>
          <nav className="site-nav">
            <NavLink to="/" end>Home</NavLink>
            <NavLink to="/open-account">Open Account</NavLink>
            <a
              href="https://www.laxmisunrise.com"
              target="_blank"
              rel="noopener noreferrer"
              style={{ fontSize: '0.9rem' }}
            >
              laxmisunrise.com
            </a>
          </nav>
        </div>
      </header>

      <main>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/open-account" element={<OpenAccount />} />
        </Routes>
      </main>

      <footer className="site-footer">
        <div className="container" style={{ padding: '0 1.5rem' }}>
          <strong>Laxmi Sunrise Bank</strong> — Aspire. Together.
          <br />
          Digital Identity Verification Demo
        </div>
      </footer>
    </div>
  );
}

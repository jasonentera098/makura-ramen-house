import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Header from './Components/Header';
import Footer from './Components/Footer';
import Home from './pages/Home';
import About from './pages/About';
import Menu from './pages/Menu';
import Promo from './pages/Promo';
import Contact from './pages/Contact';
import Login from './pages/Login';
import Signup from './pages/Signup';
import Order from './pages/Order';

function App() {
  return (
    <Router>
      <Routes>
        {/* Routes with Header and Footer */}
        <Route
          path="/"
          element={
            <>
              <Header />
              <Home />
              <Footer />
            </>
          }
        />
        <Route
          path="/about"
          element={
            <>
              <Header />
              <About />
              <Footer />
            </>
          }
        />
        <Route
          path="/menu"
          element={
            <>
              <Header />
              <Menu />
              <Footer />
            </>
          }
        />
        <Route
          path="/promo"
          element={
            <>
              <Header />
              <Promo />
              <Footer />
            </>
          }
        />
        <Route
          path="/contact"
          element={
            <>
              <Header />
              <Contact />
              <Footer />
            </>
          }
        />
        <Route
          path="/order"
          element={
            <>
              <Header />
              <Order />
              <Footer />
            </>
          }
        />

        {/* Routes without Header and Footer */}
        <Route path="/login" element={<Login />} />
        <Route path="/signup" element={<Signup />} />
      </Routes>
    </Router>
  );
}

export default App;
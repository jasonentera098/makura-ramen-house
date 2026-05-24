import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

const Header = () => {
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 50);
    };

    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  const toggleMobileMenu = () => {
    setIsMobileMenuOpen(!isMobileMenuOpen);
  };

  return (
    <>
      <header
        className={`fixed top-0 left-0 right-0 w-full z-[99] flex items-center justify-between py-[10px] px-[20px] sm:py-[15px] sm:px-[37px] sm:pl-[41px] box-border isolate gap-5 min-h-[60px] sm:min-h-[100px] transition-all duration-300 text-center text-[25px] text-firebrick font-montserrat ${
          isScrolled
            ? 'bg-[rgba(242,223,116,0.568)] backdrop-blur-[1px]'
            : 'bg-gradient-golden'
        }`}
      >
        {/* Logo and Brand */}
        <div className="frame-parent flex items-center gap-0 w-[200px] sm:w-[380px] max-w-full shrink-0">
          <div className="frame-wrapper flex flex-col items-center p-0">
            <img
              src="/public/Group-39@2x.png"
              alt="Logo"
              className="frame-child w-[40px] h-[40px] sm:w-[85px] sm:h-[85px] md:w-[50px] md:h-[50px] lg:w-[85px] lg:h-[85px] relative object-cover z-[1]"
            />
          </div>
          <div className="ramen-house relative z-[1] h-auto flex-1 font-extrabold flex items-center max-w-full ml-[5px] sm:ml-[10px]">
            <span className="text-[14px] sm:text-[20px] bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent">
              RAMEN HOUSE
            </span>
          </div>
        </div>

        {/* Desktop Navigation */}
        <div className="navigation hidden lg:flex flex-col justify-center items-start p-0 box-border shrink-0 max-w-full text-[20px]">
          <div className="navigation-items-parent self-stretch flex items-center gap-[9px] max-w-full">
            <div className="navigation-items flex items-center gap-8 shrink-0">
              <h3 className="home m-0 relative text-inherit font-extrabold font-inherit bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent cursor-pointer z-[1]">
                <Link to="/">Home</Link>
              </h3>
              <h3 className="m-0 relative text-inherit font-extrabold font-inherit bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent cursor-pointer z-[1]">
                <Link to="/about">About</Link>
              </h3>
              <h3 className="menu m-0 relative text-inherit font-extrabold font-inherit bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent cursor-pointer z-[1]">
                <Link to="/menu">Menu</Link>
              </h3>
              <h3 className="menu m-0 relative text-inherit font-extrabold font-inherit bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent cursor-pointer z-[1]">
                <Link to="/promo">Promo</Link>
              </h3>
              <h3 className="m-0 relative text-inherit font-extrabold font-inherit bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent cursor-pointer z-[1]">
                <Link to="/order">Order</Link>
              </h3>
              <h3 className="contacts m-0 relative text-inherit font-extrabold font-inherit bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent cursor-pointer z-[1]">
                <Link to="/contact">Contacts</Link>
              </h3>
            </div>
            <Link
              to="/login"
              className="image-3-parent border-0 p-0 bg-transparent flex items-center gap-1 cursor-pointer shrink-0 z-[1]"
            >
              <img
                src="/public/image-3@2x.png"
                alt="User"
                className="image-3-icon w-[25px] h-[25px] relative object-cover shrink-0"
              />
              <div className="log-in-wrapper flex flex-col items-start justify-center p-0 box-border shrink-0">
                <button className="log-in cursor-pointer border-0 p-0 bg-transparent relative text-[20px] font-extrabold font-montserrat bg-gradient-to-r from-firebrick to-black bg-clip-text text-transparent text-center inline-block shrink-0 whitespace-nowrap">
                  Log In
                </button>
              </div>
            </Link>
          </div>
        </div>

        {/* Burger Menu Icon */}
        <button
          onClick={toggleMobileMenu}
          className={`burger-menu lg:hidden flex flex-col justify-between w-[25px] h-[20px] sm:w-[30px] sm:h-[24px] bg-transparent border-none cursor-pointer p-0 z-[1001] ${
            isMobileMenuOpen ? 'active' : ''
          }`}
        >
          <span
            className={`burger-line w-full h-[2.5px] sm:h-[3px] bg-golden rounded-sm transition-all duration-300 ${
              isMobileMenuOpen ? 'rotate-45 translate-y-[8.5px] sm:translate-y-[10.5px]' : ''
            }`}
          ></span>
          <span
            className={`burger-line w-full h-[2.5px] sm:h-[3px] bg-golden rounded-sm transition-all duration-300 ${
              isMobileMenuOpen ? 'opacity-0' : ''
            }`}
          ></span>
          <span
            className={`burger-line w-full h-[2.5px] sm:h-[3px] bg-golden rounded-sm transition-all duration-300 ${
              isMobileMenuOpen ? '-rotate-45 -translate-y-[8.5px] sm:-translate-y-[10.5px]' : ''
            }`}
          ></span>
        </button>

        {/* Bottom Border */}
        <img
          src="/public/Group-47.svg"
          alt=""
          className="header-item absolute right-0 bottom-[-1px] left-0 h-[3px] w-full max-w-full overflow-hidden z-[2] shrink-0"
        />
      </header>

      {/* Mobile Menu */}
      <div
        className={`mobile-menu lg:hidden fixed top-[60px] sm:top-[80px] w-[200px] sm:w-[250px] h-[calc(100vh-60px)] sm:h-[calc(100vh-80px)] bg-black flex flex-col p-6 sm:p-10 gap-6 sm:gap-8 z-[1000] transition-all duration-300 border-l-2 border-golden/30 ${
          isMobileMenuOpen ? 'right-0' : '-right-full'
        }`}
      >
        <h3 className="mobile-menu-item m-0 text-[16px] sm:text-[20px] font-semibold font-montserrat text-white cursor-pointer transition-all duration-300 py-2 sm:py-2.5 border-b border-golden/20 hover:text-golden hover:translate-x-2.5">
          <Link to="/" onClick={toggleMobileMenu}>Home</Link>
        </h3>
        <h3 className="mobile-menu-item m-0 text-[16px] sm:text-[20px] font-semibold font-montserrat text-white cursor-pointer transition-all duration-300 py-2 sm:py-2.5 border-b border-golden/20 hover:text-golden hover:translate-x-2.5">
          <Link to="/about" onClick={toggleMobileMenu}>About</Link>
        </h3>
        <h3 className="mobile-menu-item m-0 text-[16px] sm:text-[20px] font-semibold font-montserrat text-white cursor-pointer transition-all duration-300 py-2 sm:py-2.5 border-b border-golden/20 hover:text-golden hover:translate-x-2.5">
          <Link to="/menu" onClick={toggleMobileMenu}>Menu</Link>
        </h3>
        <h3 className="mobile-menu-item m-0 text-[16px] sm:text-[20px] font-semibold font-montserrat text-white cursor-pointer transition-all duration-300 py-2 sm:py-2.5 border-b border-golden/20 hover:text-golden hover:translate-x-2.5">
          <Link to="/promo" onClick={toggleMobileMenu}>Promo</Link>
        </h3>
        <h3 className="mobile-menu-item m-0 text-[16px] sm:text-[20px] font-semibold font-montserrat text-white cursor-pointer transition-all duration-300 py-2 sm:py-2.5 border-b border-golden/20 hover:text-golden hover:translate-x-2.5">
          <Link to="/order" onClick={toggleMobileMenu}>Order</Link>
        </h3>
        <h3 className="mobile-menu-item m-0 text-[16px] sm:text-[20px] font-semibold font-montserrat text-white cursor-pointer transition-all duration-300 py-2 sm:py-2.5 border-b border-golden/20 hover:text-golden hover:translate-x-2.5">
          <Link to="/contact" onClick={toggleMobileMenu}>Contacts</Link>
        </h3>
        <button 
          onClick={toggleMobileMenu}
          className="mobile-login-btn px-4 py-2 sm:px-6 sm:py-3 bg-gradient-to-r from-firebrick to-golden border-none rounded-[10px] text-[14px] sm:text-[16px] font-bold font-poppins text-white cursor-pointer transition-all duration-300 mt-3 sm:mt-5 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.4)]"
        >
          <Link to="/login">Log In</Link>
        </button>
      </div>
    </>
  );
};

export default Header;

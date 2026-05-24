import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';

const Login = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [showPassword, setShowPassword] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Login submitted:', formData);
    navigate('/');
  };

  return (
    <div className="w-full min-h-screen bg-black flex items-center justify-center p-4">
      <div className="w-full max-w-[1200px] grid grid-cols-1 lg:grid-cols-2 bg-white/5 rounded-[30px] overflow-hidden shadow-[0_20px_60px_rgba(0,0,0,0.5)] backdrop-blur-sm border border-golden/20">
        {/* Left Side - Login Form */}
        <div className="p-12 flex flex-col justify-center lg:p-10 md:p-8 sm:p-6">
          {/* Logo */}
          <div className="flex flex-col items-center mb-8">
            <img
              src="/public/Group-39@2x.png"
              alt="Logo"
              className="w-[100px] h-[100px] object-cover mb-4 md:w-[80px] md:h-[80px]"
            />
            <h1 className="text-[32px] font-extrabold font-montserrat text-gradient lg:text-[28px] md:text-[24px]">
              RAMEN HOUSE
            </h1>
          </div>

          {/* Login Form */}
          <div className="w-full">
            <h2 className="text-[36px] font-bold font-poppins text-white mb-2 lg:text-[32px] md:text-[28px]">
              Welcome Back
            </h2>
            <p className="text-[16px] font-montserrat text-gray-400 mb-8 lg:text-[15px]">
              Please login to your account
            </p>

            <form onSubmit={handleSubmit} className="flex flex-col gap-6">
              {/* Email Input */}
              <div className="relative">
                <label className="block text-[16px] font-montserrat text-white mb-2 lg:text-[15px]">
                  Email
                </label>
                <div className="relative">
                  <span className="absolute left-4 top-1/2 -translate-y-1/2 text-golden text-[20px]">
                    ✉️
                  </span>
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    placeholder="Enter your email"
                    className="w-full pl-12 pr-4 py-4 bg-black/50 border border-golden/30 rounded-[15px] text-white font-montserrat focus:outline-none focus:border-golden transition-colors placeholder:text-gray-500"
                    required
                  />
                </div>
              </div>

              {/* Password Input */}
              <div className="relative">
                <label className="block text-[16px] font-montserrat text-white mb-2 lg:text-[15px]">
                  Password
                </label>
                <div className="relative">
                  <span className="absolute left-4 top-1/2 -translate-y-1/2 text-golden text-[20px]">
                    🔒
                  </span>
                  <input
                    type={showPassword ? 'text' : 'password'}
                    name="password"
                    value={formData.password}
                    onChange={handleChange}
                    placeholder="Enter your password"
                    className="w-full pl-12 pr-12 py-4 bg-black/50 border border-golden/30 rounded-[15px] text-white font-montserrat focus:outline-none focus:border-golden transition-colors placeholder:text-gray-500"
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-4 top-1/2 -translate-y-1/2 text-golden text-[20px] cursor-pointer hover:text-golden-dark transition-colors"
                  >
                    {showPassword ? '👁️' : '👁️‍🗨️'}
                  </button>
                </div>
              </div>

              {/* Remember Me & Forgot Password */}
              <div className="flex items-center justify-between">
                <label className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="checkbox"
                    className="w-4 h-4 accent-golden cursor-pointer"
                  />
                  <span className="text-[14px] font-montserrat text-gray-400">
                    Remember me
                  </span>
                </label>
                <a
                  href="#"
                  className="text-[14px] font-montserrat text-golden hover:text-golden-dark transition-colors"
                >
                  Forgot Password?
                </a>
              </div>

              {/* Login Button */}
              <button
                type="submit"
                className="w-full px-8 py-4 bg-gradient-golden text-black text-[20px] font-bold font-poppins border-none rounded-[15px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] lg:text-[18px]"
              >
                Log In
              </button>

              {/* Sign Up Link */}
              <p className="text-center text-[16px] font-montserrat text-gray-400 lg:text-[15px]">
                Don't have an account?{' '}
                <Link
                  to="/signup"
                  className="text-golden font-semibold hover:text-golden-dark transition-colors"
                >
                  Sign Up
                </Link>
              </p>
            </form>
          </div>
        </div>

        {/* Right Side - Image */}
        <div className="hidden lg:block relative">
          <img
            src="/public/background.png"
            alt="Ramen"
            className="w-full h-full object-cover"
          />
          <div className="absolute inset-0 bg-gradient-to-l from-transparent to-black/20"></div>
        </div>
      </div>
    </div>
  );
};

export default Login;

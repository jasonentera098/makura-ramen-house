import { useState } from 'react';
import emailjs from '@emailjs/browser';

const Order = () => {
  // Menu items organized by category
  const menuItems = {
    ramen: [
      { id: 'r1', name: 'Tonkotsu Ramen', price: 139, image: '/public/pic@2x.png' },
      { id: 'r2', name: 'Katsu Curry Ramen', price: 149, image: '/public/Group-38@2x.png' },
      { id: 'r3', name: 'Cheesy Tonkotsu', price: 159, image: '/public/Untitled-design-2-2@2x.png' },
      { id: 'r4', name: 'Red Ramen', price: 149, image: '/public/Untitled-design-3-1@2x.png' },
    ],
    budgetMeals: [
      { id: 'b1', name: 'Budget Meal 1', price: 89, image: '/public/b.png' },
      { id: 'b2', name: 'Budget Meal 2', price: 89, image: '/public/b1.png' },
      { id: 'b3', name: 'Budget Meal 3', price: 89, image: '/public/b2.png' },
      { id: 'b4', name: 'Budget Meal 4', price: 89, image: '/public/b3.png' },
      { id: 'b5', name: 'Budget Meal 5', price: 89, image: '/public/b5.png' },
    ],
    platter: [
      { id: 'p1', name: 'Platter 1', price: 199, image: '/public/p1.png' },
      { id: 'p2', name: 'Platter 2', price: 199, image: '/public/p3.png' },
      { id: 'p3', name: 'Platter 3', price: 199, image: '/public/p4.png' },
      { id: 'p4', name: 'Platter 4', price: 199, image: '/public/p5.png' },
    ],
    snacks: [
      { id: 's1', name: 'Snack 1', price: 49, image: '/public/S1.png' },
      { id: 's2', name: 'Snack 2', price: 49, image: '/public/S2.png' },
      { id: 's3', name: 'Snack 3', price: 49, image: '/public/S3.png' },
      { id: 's4', name: 'Snack 4', price: 49, image: '/public/S4.png' },
    ],
    unlimitedPromos: [
      { id: 'up1', name: 'Unlimited Ramen Promo', price: 399, image: '/public/unli1@2x.png' },
      { id: 'up2', name: 'Unlimited Rice & Sides', price: 299, image: '/public/unli2@2x.png' },
      { id: 'up3', name: 'Weekend Special (20% OFF)', price: 0, image: '/public/648511596-1533822838745647-5485652144745721927-n-1@2x.png' },
    ],
    barkadaFeast: [
      { id: 'bf1', name: 'Family Feast Bundle', price: 1499, image: '/public/image-2@2x.png' },
      { id: 'bf2', name: 'Party Platter', price: 1999, image: '/public/image-31@2x.png' },
      { id: 'bf3', name: 'Ultimate Barkada Bundle', price: 2499, image: '/public/image-1@2x.png' },
    ],
  };

  const [cart, setCart] = useState([]);
  const [customerInfo, setCustomerInfo] = useState({
    name: '',
    email: '',
    phone: '',
    address: '',
    notes: '',
  });

  // Add item to cart
  const addToCart = (item) => {
    const existingItem = cart.find((cartItem) => cartItem.id === item.id);
    if (existingItem) {
      setCart(
        cart.map((cartItem) =>
          cartItem.id === item.id
            ? { ...cartItem, quantity: cartItem.quantity + 1 }
            : cartItem
        )
      );
    } else {
      setCart([...cart, { ...item, quantity: 1 }]);
    }
  };

  // Remove item from cart
  const removeFromCart = (itemId) => {
    setCart(cart.filter((item) => item.id !== itemId));
  };

  // Update quantity
  const updateQuantity = (itemId, newQuantity) => {
    if (newQuantity <= 0) {
      removeFromCart(itemId);
    } else {
      setCart(
        cart.map((item) =>
          item.id === itemId ? { ...item, quantity: newQuantity } : item
        )
      );
    }
  };

  // Calculate total
  const calculateTotal = () => {
    return cart.reduce((total, item) => total + item.price * item.quantity, 0);
  };

  // Handle form input change
  const handleInputChange = (e) => {
    setCustomerInfo({
      ...customerInfo,
      [e.target.name]: e.target.value,
    });
  };

  // Handle order submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (cart.length === 0) {
      alert('Please add items to your cart before ordering.');
      return;
    }
    if (!customerInfo.name || !customerInfo.email || !customerInfo.phone || !customerInfo.address) {
      alert('Please fill in all required fields.');
      return;
    }

    // Prepare order details for email
    let orderDetails = 'ORDER DETAILS:\n\n';
    cart.forEach((item) => {
      orderDetails += `${item.name} x${item.quantity} - ₱${item.price * item.quantity}\n`;
    });
    orderDetails += `\nTOTAL: ₱${calculateTotal()}\n\n`;
    orderDetails += `CUSTOMER INFORMATION:\n`;
    orderDetails += `Name: ${customerInfo.name}\n`;
    orderDetails += `Email: ${customerInfo.email}\n`;
    orderDetails += `Phone: ${customerInfo.phone}\n`;
    orderDetails += `Address: ${customerInfo.address}\n`;
    if (customerInfo.notes) {
      orderDetails += `Notes: ${customerInfo.notes}\n`;
    }

    // EmailJS configuration
    const serviceID = 'service_7bq4ci4'; 
    const templateID = 'template_pfl13zc'; 
    const publicKey = '4bMmTkhS5P-kQ9np_'; 

    const templateParams = {
      to_email: 'jasonentera14@gmail.com',
      from_name: customerInfo.name,
      from_email: customerInfo.email,
      phone: customerInfo.phone,
      address: customerInfo.address,
      order_details: orderDetails,
      total: calculateTotal(),
    };

    try {
      await emailjs.send(serviceID, templateID, templateParams, publicKey);
      alert('Order placed successfully! We will contact you soon.');
      // Reset cart and form
      setCart([]);
      setCustomerInfo({
        name: '',
        email: '',
        phone: '',
        address: '',
        notes: '',
      });
    } catch (error) {
      console.error('Error sending email:', error);
      alert('Failed to place order. Please try again or contact us directly at 09534879391.');
    }
  };

  return (
    <div className="w-full min-h-screen bg-black text-white pt-[100px] pb-12">
      <div className="max-w-[1440px] mx-auto px-8 lg:px-6 md:px-5 sm:px-4">
        {/* Page Title */}
        <div className="text-center mb-12 md:mb-10 sm:mb-8 mt-[50px]">
          <h1 className="text-[22px] sm:text-[28px] md:text-[32px] lg:text-[40px] font-bold font-poppins text-golden text-shadow-golden">
            Order Now
          </h1>
          <p className="text-[13px] sm:text-[14px] md:text-[16px] text-white/80 mt-2 font-montserrat">
            Select your favorite items and place your order
          </p>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8 lg:gap-6">
          {/* Menu Items Section */}
          <div className="lg:col-span-2">
            {/* Ramen Section */}
            <section className="mb-10 md:mb-8 sm:mb-6">
              <h2 className="text-[18px] sm:text-[20px] md:text-[24px] font-bold font-poppins text-golden mb-4">
                Ramen Bowls
              </h2>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-3">
                {menuItems.ramen.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#1a1a1a] rounded-[15px] overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105 hover:shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                  >
                    <img
                      src={item.image}
                      alt={item.name}
                      className="w-full h-[120px] sm:h-[140px] md:h-[160px] object-contain bg-[#d3d3d3] p-2"
                    />
                    <div className="p-3 sm:p-4">
                      <h3 className="text-[11px] sm:text-[12px] md:text-[13px] font-bold text-white mb-2">
                        {item.name}
                      </h3>
                      <p className="text-[13px] sm:text-[14px] md:text-[16px] font-bold text-golden mb-3">
                        ₱{item.price}
                      </p>
                      <button
                        onClick={() => addToCart(item)}
                        className="w-full bg-golden text-black text-[11px] sm:text-[12px] md:text-[13px] font-bold py-2 rounded-[8px] hover:bg-golden-dark transition-colors"
                      >
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            {/* Budget Meals Section */}
            <section className="mb-10 md:mb-8 sm:mb-6">
              <h2 className="text-[18px] sm:text-[20px] md:text-[24px] font-bold font-poppins text-golden mb-4">
                Budget Meals
              </h2>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-3">
                {menuItems.budgetMeals.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#1a1a1a] rounded-[15px] overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105 hover:shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                  >
                    <img
                      src={item.image}
                      alt={item.name}
                      className="w-full h-[120px] sm:h-[140px] md:h-[160px] object-cover rounded-full p-2"
                    />
                    <div className="p-3 sm:p-4">
                      <h3 className="text-[11px] sm:text-[12px] md:text-[13px] font-bold text-white mb-2">
                        {item.name}
                      </h3>
                      <p className="text-[13px] sm:text-[14px] md:text-[16px] font-bold text-golden mb-3">
                        ₱{item.price}
                      </p>
                      <button
                        onClick={() => addToCart(item)}
                        className="w-full bg-golden text-black text-[11px] sm:text-[12px] md:text-[13px] font-bold py-2 rounded-[8px] hover:bg-golden-dark transition-colors"
                      >
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            {/* Platter Section */}
            <section className="mb-10 md:mb-8 sm:mb-6">
              <h2 className="text-[18px] sm:text-[20px] md:text-[24px] font-bold font-poppins text-golden mb-4">
                Platter
              </h2>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-3">
                {menuItems.platter.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#1a1a1a] rounded-[15px] overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105 hover:shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                  >
                    <img
                      src={item.image}
                      alt={item.name}
                      className="w-full h-[120px] sm:h-[140px] md:h-[160px] object-cover rounded-full p-2"
                    />
                    <div className="p-3 sm:p-4">
                      <h3 className="text-[11px] sm:text-[12px] md:text-[13px] font-bold text-white mb-2">
                        {item.name}
                      </h3>
                      <p className="text-[13px] sm:text-[14px] md:text-[16px] font-bold text-golden mb-3">
                        ₱{item.price}
                      </p>
                      <button
                        onClick={() => addToCart(item)}
                        className="w-full bg-golden text-black text-[11px] sm:text-[12px] md:text-[13px] font-bold py-2 rounded-[8px] hover:bg-golden-dark transition-colors"
                      >
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            {/* Snacks Section */}
            <section className="mb-10 md:mb-8 sm:mb-6">
              <h2 className="text-[18px] sm:text-[20px] md:text-[24px] font-bold font-poppins text-golden mb-4">
                Snacks
              </h2>
              <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 sm:gap-3">
                {menuItems.snacks.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#1a1a1a] rounded-[15px] overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105 hover:shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                  >
                    <img
                      src={item.image}
                      alt={item.name}
                      className="w-full h-[120px] sm:h-[140px] md:h-[160px] object-cover rounded-full p-2"
                    />
                    <div className="p-3 sm:p-4">
                      <h3 className="text-[11px] sm:text-[12px] md:text-[13px] font-bold text-white mb-2">
                        {item.name}
                      </h3>
                      <p className="text-[13px] sm:text-[14px] md:text-[16px] font-bold text-golden mb-3">
                        ₱{item.price}
                      </p>
                      <button
                        onClick={() => addToCart(item)}
                        className="w-full bg-golden text-black text-[11px] sm:text-[12px] md:text-[13px] font-bold py-2 rounded-[8px] hover:bg-golden-dark transition-colors"
                      >
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            {/* Unlimited Promos Section */}
            <section className="mb-10 md:mb-8 sm:mb-6">
              <h2 className="text-[18px] sm:text-[20px] md:text-[24px] font-bold font-poppins text-golden mb-4">
                Unlimited Promos
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-3">
                {menuItems.unlimitedPromos.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#1a1a1a] rounded-[15px] overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105 hover:shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                  >
                    <img
                      src={item.image}
                      alt={item.name}
                      className="w-full h-[180px] sm:h-[200px] md:h-[220px] object-cover"
                    />
                    <div className="p-3 sm:p-4">
                      <h3 className="text-[12px] sm:text-[13px] md:text-[14px] font-bold text-white mb-2">
                        {item.name}
                      </h3>
                      <p className="text-[14px] sm:text-[16px] md:text-[18px] font-bold text-golden mb-3">
                        {item.price > 0 ? `₱${item.price}` : 'Special Offer'}
                      </p>
                      <button
                        onClick={() => addToCart(item)}
                        className="w-full bg-golden text-black text-[11px] sm:text-[12px] md:text-[13px] font-bold py-2 rounded-[8px] hover:bg-golden-dark transition-colors"
                      >
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>

            {/* Barkada Feast Section */}
            <section className="mb-10 md:mb-8 sm:mb-6">
              <h2 className="text-[18px] sm:text-[20px] md:text-[24px] font-bold font-poppins text-golden mb-4">
                Barkada Feast
              </h2>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-3">
                {menuItems.barkadaFeast.map((item) => (
                  <div
                    key={item.id}
                    className="bg-[#1a1a1a] rounded-[15px] overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105 hover:shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                  >
                    <img
                      src={item.image}
                      alt={item.name}
                      className="w-full h-[180px] sm:h-[200px] md:h-[220px] object-cover"
                    />
                    <div className="p-3 sm:p-4">
                      <h3 className="text-[12px] sm:text-[13px] md:text-[14px] font-bold text-white mb-2">
                        {item.name}
                      </h3>
                      <p className="text-[14px] sm:text-[16px] md:text-[18px] font-bold text-golden mb-3">
                        ₱{item.price}
                      </p>
                      <button
                        onClick={() => addToCart(item)}
                        className="w-full bg-golden text-black text-[11px] sm:text-[12px] md:text-[13px] font-bold py-2 rounded-[8px] hover:bg-golden-dark transition-colors"
                      >
                        Add to Cart
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </section>
          </div>

          {/* Cart and Customer Info Section */}
          <div className="lg:col-span-1">
            <div className="sticky top-[120px]">
              {/* Cart Summary */}
              <div className="bg-[#1a1a1a] rounded-[15px] p-5 sm:p-6 mb-6">
                <h2 className="text-[18px] sm:text-[20px] md:text-[22px] font-bold font-poppins text-golden mb-4">
                  Your Cart
                </h2>
                {cart.length === 0 ? (
                  <p className="text-[13px] sm:text-[14px] text-white/60 text-center py-8">
                    Your cart is empty
                  </p>
                ) : (
                  <>
                    <div className="max-h-[300px] overflow-y-auto mb-4 scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px]">
                      {cart.map((item) => (
                        <div
                          key={item.id}
                          className="flex items-center gap-3 mb-4 pb-4 border-b border-white/10"
                        >
                          <img
                            src={item.image}
                            alt={item.name}
                            className="w-[50px] h-[50px] object-contain bg-[#d3d3d3] rounded-[8px]"
                          />
                          <div className="flex-1">
                            <h3 className="text-[11px] sm:text-[12px] font-bold text-white">
                              {item.name}
                            </h3>
                            <p className="text-[12px] sm:text-[13px] text-golden">
                              ₱{item.price}
                            </p>
                            <div className="flex items-center gap-2 mt-1">
                              <button
                                onClick={() => updateQuantity(item.id, item.quantity - 1)}
                                className="w-[20px] h-[20px] bg-golden text-black text-[12px] font-bold rounded-[4px] hover:bg-golden-dark"
                              >
                                -
                              </button>
                              <span className="text-[12px] text-white">{item.quantity}</span>
                              <button
                                onClick={() => updateQuantity(item.id, item.quantity + 1)}
                                className="w-[20px] h-[20px] bg-golden text-black text-[12px] font-bold rounded-[4px] hover:bg-golden-dark"
                              >
                                +
                              </button>
                            </div>
                          </div>
                          <button
                            onClick={() => removeFromCart(item.id)}
                            className="text-[18px] text-red-500 hover:text-red-700"
                          >
                            ×
                          </button>
                        </div>
                      ))}
                    </div>
                    <div className="border-t border-white/20 pt-4">
                      <div className="flex justify-between items-center">
                        <span className="text-[16px] sm:text-[18px] font-bold text-white">
                          Total:
                        </span>
                        <span className="text-[18px] sm:text-[20px] font-bold text-golden">
                          ₱{calculateTotal()}
                        </span>
                      </div>
                    </div>
                  </>
                )}
              </div>

              {/* Customer Information Form */}
              <div className="bg-[#1a1a1a] rounded-[15px] p-5 sm:p-6">
                <h2 className="text-[18px] sm:text-[20px] md:text-[22px] font-bold font-poppins text-golden mb-4">
                  Customer Info
                </h2>
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div>
                    <label className="block text-[12px] sm:text-[13px] text-white mb-2">
                      Name *
                    </label>
                    <input
                      type="text"
                      name="name"
                      value={customerInfo.name}
                      onChange={handleInputChange}
                      required
                      className="w-full bg-black border border-golden/30 rounded-[8px] px-3 py-2 text-[12px] sm:text-[13px] text-white focus:outline-none focus:border-golden"
                    />
                  </div>
                  <div>
                    <label className="block text-[12px] sm:text-[13px] text-white mb-2">
                      Email *
                    </label>
                    <input
                      type="email"
                      name="email"
                      value={customerInfo.email}
                      onChange={handleInputChange}
                      required
                      className="w-full bg-black border border-golden/30 rounded-[8px] px-3 py-2 text-[12px] sm:text-[13px] text-white focus:outline-none focus:border-golden"
                    />
                  </div>
                  <div>
                    <label className="block text-[12px] sm:text-[13px] text-white mb-2">
                      Phone *
                    </label>
                    <input
                      type="tel"
                      name="phone"
                      value={customerInfo.phone}
                      onChange={handleInputChange}
                      required
                      className="w-full bg-black border border-golden/30 rounded-[8px] px-3 py-2 text-[12px] sm:text-[13px] text-white focus:outline-none focus:border-golden"
                    />
                  </div>
                  <div>
                    <label className="block text-[12px] sm:text-[13px] text-white mb-2">
                      Delivery Address *
                    </label>
                    <textarea
                      name="address"
                      value={customerInfo.address}
                      onChange={handleInputChange}
                      required
                      rows="3"
                      className="w-full bg-black border border-golden/30 rounded-[8px] px-3 py-2 text-[12px] sm:text-[13px] text-white focus:outline-none focus:border-golden resize-none"
                    />
                  </div>
                  <div>
                    <label className="block text-[12px] sm:text-[13px] text-white mb-2">
                      Order Notes (Optional)
                    </label>
                    <textarea
                      name="notes"
                      value={customerInfo.notes}
                      onChange={handleInputChange}
                      rows="2"
                      className="w-full bg-black border border-golden/30 rounded-[8px] px-3 py-2 text-[12px] sm:text-[13px] text-white focus:outline-none focus:border-golden resize-none"
                    />
                  </div>
                  <button
                    type="submit"
                    className="w-full bg-golden text-black text-[14px] sm:text-[16px] font-bold py-3 rounded-[10px] hover:bg-golden-dark transition-colors"
                  >
                    Place Order
                  </button>
                </form>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Order;

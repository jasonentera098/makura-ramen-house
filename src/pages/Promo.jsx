import { useNavigate } from 'react-router-dom';

const Promo = () => {
  const navigate = useNavigate();

  const unlimitedPromos = [
    {
      name: 'Unlimited Ramen Promo',
      description: 'Enjoy unlimited servings of your favorite ramen! Perfect for ramen lovers who can\'t get enough.',
      price: '₱399',
      deadline: 'December 31, 2024',
      image: '/public/unli1@2x.png',
    },
    {
      name: 'Unlimited Rice & Sides',
      description: 'Get unlimited rice and sides with any ramen order. The perfect combo for big appetites!',
      price: '₱299',
      deadline: 'December 31, 2024',
      image: '/public/unli2@2x.png',
    },
    {
      name: 'Weekend Special',
      description: 'Exclusive weekend offer! Get 20% off on all ramen bowls every Saturday and Sunday.',
      price: '20% OFF',
      deadline: 'Every Weekend',
      image: '/public/unli1@2x.png',
    },
  ];

  const barkadaFeast = [
    {
      name: 'Family Feast Bundle',
      description: 'Perfect for 4-6 people! Includes assorted ramen bowls, appetizers, and drinks.',
      price: '₱1,499',
      deadline: 'December 31, 2024',
      image: '/public/image-2@2x.png',
    },
    {
      name: 'Party Platter',
      description: 'Great for celebrations! Serves 6-8 people with a variety of ramen and side dishes.',
      price: '₱1,999',
      deadline: 'December 31, 2024',
      image: '/public/image-31@2x.png',
    },
    {
      name: 'Ultimate Barkada Bundle',
      description: 'The ultimate feast for large groups! Serves 8-10 people with premium selections.',
      price: '₱2,499',
      deadline: 'December 31, 2024',
      image: '/public/image-1@2x.png',
    },
  ];

  return (
    <div className="w-full min-h-screen bg-black text-white pt-[150px] pb-20 px-12 lg:px-10 md:px-8 sm:px-5">
      <div className="max-w-[1440px] mx-auto">
        {/* Page Header */}
        <div className="w-full flex flex-col items-center gap-5 text-center mb-16 md:mb-12">
          <h1 className="text-[22px] font-extrabold font-poppins text-white text-shadow-golden sm:text-[28px] md:text-[32px] lg:text-[40px]">
            Special Promos & Offers
          </h1>
          <p className="text-[13px] font-medium font-montserrat text-white/80 max-w-[800px] sm:text-[14px] md:text-[16px] lg:text-[20px]">
            Don't miss out on our amazing deals and limited-time offers!
          </p>
        </div>

        {/* Unlimited Promos Section */}
        <section className="w-full flex flex-col gap-10 mb-16 md:mb-12">
          <h2 className="text-[18px] font-bold font-poppins text-white text-shadow-golden text-left sm:text-[20px] md:text-[22px] lg:text-[36px]">
            Unlimited Promos
          </h2>
          <div className="w-full grid grid-cols-1 lg:grid-cols-3 md:grid-cols-2 gap-10 justify-items-center lg:gap-8 md:gap-6">
            {unlimitedPromos.map((promo, index) => (
              <div
                key={index}
                className="w-full max-w-[450px] bg-white/5 rounded-[20px] overflow-hidden shadow-[0_8px_30px_rgba(0,0,0,0.4)] transition-all duration-300 hover:-translate-y-2.5 hover:shadow-[0_15px_40px_rgba(242,224,116,0.4)] cursor-pointer flex flex-col lg:max-w-[400px] md:max-w-full"
              >
                <div className="w-full h-[200px] overflow-hidden relative bg-black/30 sm:h-[250px] md:h-[280px] lg:h-[250px]">
                  <img
                    src={promo.image}
                    alt={promo.name}
                    className="w-full h-full object-cover transition-transform duration-300 hover:scale-110"
                  />
                </div>
                <div className="p-4 flex flex-col gap-3 flex-1 sm:p-5 md:p-5 lg:p-6">
                  <h3 className="text-[15px] font-bold font-poppins text-white text-shadow-golden sm:text-[18px] md:text-[20px] lg:text-[22px]">
                    {promo.name}
                  </h3>
                  <p className="text-[12px] font-normal font-montserrat text-white/80 leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[15px]">
                    {promo.description}
                  </p>
                  <div className="flex flex-col gap-2 py-3 border-t border-b border-golden/30">
                    <div className="text-[22px] font-extrabold font-poppins text-golden text-shadow-golden sm:text-[26px] md:text-[25px] lg:text-[28px]">
                      {promo.price}
                    </div>
                    <div className="flex flex-col gap-0.5">
                      <span className="text-[11px] font-medium font-montserrat text-white/60 uppercase tracking-wider sm:text-[12px] md:text-[14px]">
                        Valid Until:
                      </span>
                      <span className="text-[12px] font-semibold font-montserrat text-white sm:text-[14px] md:text-[16px]">
                        {promo.deadline}
                      </span>
                    </div>
                  </div>
                  <button 
                    onClick={() => navigate('/order')}
                    className="w-full px-4 py-2.5 bg-gradient-golden text-black text-[13px] font-bold font-poppins border-none rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:text-[15px] sm:py-3 md:text-[16px] lg:text-[18px] lg:px-6 lg:py-3"
                  >
                    Order Now
                  </button>
                </div>
              </div>
            ))}
          </div>
        </section>

        {/* Barkada Feast Section */}
        <section className="w-full flex flex-col gap-10">
          <h2 className="text-[18px] font-bold font-poppins text-white text-shadow-golden text-left sm:text-[20px] md:text-[22px] lg:text-[36px]">
            Barkada Feast
          </h2>
          <div className="w-full grid grid-cols-1 lg:grid-cols-3 md:grid-cols-2 gap-10 justify-items-center lg:gap-8 md:gap-6">
            {barkadaFeast.map((promo, index) => (
              <div
                key={index}
                className="w-full max-w-[450px] bg-white/5 rounded-[20px] overflow-hidden shadow-[0_8px_30px_rgba(0,0,0,0.4)] transition-all duration-300 hover:-translate-y-2.5 hover:shadow-[0_15px_40px_rgba(242,224,116,0.4)] cursor-pointer flex flex-col lg:max-w-[400px] md:max-w-full"
              >
                <div className="w-full h-[200px] overflow-hidden relative bg-black/30 sm:h-[250px] md:h-[280px] lg:h-[250px]">
                  <img
                    src={promo.image}
                    alt={promo.name}
                    className="w-full h-full object-cover transition-transform duration-300 hover:scale-110"
                  />
                </div>
                <div className="p-4 flex flex-col gap-3 flex-1 sm:p-5 md:p-5 lg:p-6">
                  <h3 className="text-[15px] font-bold font-poppins text-white text-shadow-golden sm:text-[18px] md:text-[20px] lg:text-[22px]">
                    {promo.name}
                  </h3>
                  <p className="text-[12px] font-normal font-montserrat text-white/80 leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[15px]">
                    {promo.description}
                  </p>
                  <div className="flex flex-col gap-2 py-3 border-t border-b border-golden/30">
                    <div className="text-[22px] font-extrabold font-poppins text-golden text-shadow-golden sm:text-[26px] md:text-[25px] lg:text-[28px]">
                      {promo.price}
                    </div>
                    <div className="flex flex-col gap-0.5">
                      <span className="text-[11px] font-medium font-montserrat text-white/60 uppercase tracking-wider sm:text-[12px] md:text-[14px]">
                        Valid Until:
                      </span>
                      <span className="text-[12px] font-semibold font-montserrat text-white sm:text-[14px] md:text-[16px]">
                        {promo.deadline}
                      </span>
                    </div>
                  </div>
                  <button 
                    onClick={() => navigate('/order')}
                    className="w-full px-4 py-2.5 bg-gradient-golden text-black text-[13px] font-bold font-poppins border-none rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:text-[15px] sm:py-3 md:text-[16px] lg:text-[18px] lg:px-6 lg:py-3"
                  >
                    Order Now
                  </button>
                </div>
              </div>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
};

export default Promo;

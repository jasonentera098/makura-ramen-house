import { useNavigate } from 'react-router-dom';

const Menu = () => {
  const navigate = useNavigate();

  const menuCategories = [
    { name: 'Platter', image: '/public/Platter.png' },
    { name: 'Bento Meals', image: '/public/BentoMeals.png' },
    { name: 'Budget Meals', image: '/public/budgetmeal.png' },
    { name: 'Snacks', image: '/public/Snacks.png' },
  ];

  const budgetMeals = [
    '/public/b.png',
    '/public/b1.png',
    '/public/b2.png',
    '/public/b3.png',
    '/public/b1.png',
    '/public/b5.png',
  ];

  const platters = [
    '/public/p1.png',
    '/public/p3.png',
    '/public/p4.png',
    '/public/p5.png',
    '/public/p6.png',
    '/public/p7.png',
  ];

  const snacks = [
    '/public/S1.png',
    '/public/S2.png',
    '/public/S3.png',
    '/public/S4.png',
    '/public/S5.png',
    '/public/S6.png',
    '/public/S7.png',
    '/public/S8.png',
  ];

  const bentoMeals = [
    '/public/d1.png',
    '/public/d7.png',
    '/public/d3.png',
    '/public/d4.png',
    '/public/d5.png',
    '/public/d6.png',
    '/public/d7.png',
  ];

  const ramenBowls = [
    '/public/R1.png',
    '/public/R2.png',
    '/public/R3.png',
    '/public/R4.png',
    
  ];

  return (
    <div className="w-full min-h-screen bg-white text-white pt-[100px] pb-12">
      <div className="max-w-[1440px] mx-auto px-8 lg:px-6 md:px-4">
        {/* Page Title */}
        <div className="text-center mb-16 md:mb-12">
          <h1 className="pt-[50px] text-[22px] font-bold font-poppins text-black text-shadow-golden mb-4 sm:text-[28px] md:text-[32px] lg:text-[40px]">
            Makura Menu
          </h1>
        
        </div>

        {/* Menu Categories */}
        <section className="mb-20 md:mb-16">
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-8 lg:gap-6 md:grid-cols-1 md:gap-4 tablet-menu-categories">
            {menuCategories.map((category, index) => (
              <div
                key={index}
                className="relative rounded-[20px] overflow-hidden group cursor-pointer transition-all duration-300 hover:-translate-y-2 hover:shadow-[0_15px_40px_rgba(242,224,116,0.4)] tablet-menu-item"
              >
                <img
                  src={category.image}
                  alt={category.name}
                  className="w-full h-[300px] object-cover lg:h-[350px] md:h-[250px]"
                />
                <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/40 to-transparent flex items-end p-6 lg:p-4">
                  <h3 className="text-[24px] font-bold font-poppins text-golden lg:text-[22px] md:text-[18px]">
                    {category.name}
                  </h3>
                </div>
              </div>
            ))}
          </div>
        </section>

        {/* Budget Meals Section */}
        <section className="mb-20 md:mb-16 bg-black rounded-[20px] p-10 lg:p-8 md:p-6 sm:p-4">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-[18px] font-bold font-poppins text-shadow-golden sm:text-[20px] md:text-[24px] lg:text-[36px]">
              Budget Meals
            </h2>
            <button
              onClick={() => navigate('/order')}
              className="px-4 py-2 bg-gradient-golden text-black text-[12px] font-bold font-poppins rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:px-5 sm:py-2.5 sm:text-[14px] md:text-[15px] lg:px-6 lg:py-3 lg:text-[16px]"
            >
              Order Now
            </button>
          </div>
          <div className="w-full flex flex-row gap-[30px] overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px] hover:scrollbar-thumb-golden/80 md:gap-4 sm:gap-3">
            {budgetMeals.map((image, index) => (
              <div
                key={index}
                className="min-w-[130px] w-[130px] sm:min-w-[160px] sm:w-[160px] md:min-w-[180px] md:w-[180px] lg:min-w-[250px] lg:w-[250px] h-auto bg-transparent rounded-full overflow-visible flex-shrink-0 transition-all duration-300 hover:scale-110 cursor-pointer relative"
              >
                <img
                  src={image}
                  alt={`Budget Meal ${index + 1}`}
                  className="w-full h-auto block object-cover rounded-full shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                />
              </div>
            ))}
          </div>
        </section>

        {/* Platter Section */}
        <section className="mb-20 md:mb-16 bg-black rounded-[20px] p-10 lg:p-8 md:p-6 sm:p-4">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-[18px] font-bold font-poppins text-shadow-golden sm:text-[20px] md:text-[24px] lg:text-[36px]">
              Platter
            </h2>
            <button
              onClick={() => navigate('/order')}
              className="px-4 py-2 bg-gradient-golden text-black text-[12px] font-bold font-poppins rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:px-5 sm:py-2.5 sm:text-[14px] md:text-[15px] lg:px-6 lg:py-3 lg:text-[16px]"
            >
              Order Now
            </button>
          </div>
          <div className="w-full flex flex-row gap-[30px] overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px] hover:scrollbar-thumb-golden/80 md:gap-4 sm:gap-3">
            {platters.map((image, index) => (
              <div
                key={index}
                className="min-w-[130px] w-[130px] sm:min-w-[160px] sm:w-[160px] md:min-w-[180px] md:w-[180px] lg:min-w-[250px] lg:w-[250px] h-auto bg-transparent rounded-full overflow-visible flex-shrink-0 transition-all duration-300 hover:scale-110 cursor-pointer relative"
              >
                <img
                  src={image}
                  alt={`Platter ${index + 1}`}
                  className="w-full h-auto block object-cover rounded-full shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                />
              </div>
            ))}
          </div>
        </section>

        {/* Snacks Section */}
        <section className="mb-20 md:mb-16 bg-black rounded-[20px] p-10 lg:p-8 md:p-6 sm:p-4">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-[18px] font-bold font-poppins text-shadow-golden sm:text-[20px] md:text-[24px] lg:text-[36px]">
              Snacks
            </h2>
            <button
              onClick={() => navigate('/order')}
              className="px-4 py-2 bg-gradient-golden text-black text-[12px] font-bold font-poppins rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:px-5 sm:py-2.5 sm:text-[14px] md:text-[15px] lg:px-6 lg:py-3 lg:text-[16px]"
            >
              Order Now
            </button>
          </div>
          <div className="w-full flex flex-row gap-[30px] overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px] hover:scrollbar-thumb-golden/80 md:gap-4 sm:gap-3">
            {snacks.map((image, index) => (
              <div
                key={index}
                className="min-w-[130px] w-[130px] sm:min-w-[160px] sm:w-[160px] md:min-w-[180px] md:w-[180px] lg:min-w-[250px] lg:w-[250px] h-auto bg-transparent rounded-full overflow-visible flex-shrink-0 transition-all duration-300 hover:scale-110 cursor-pointer relative"
              >
                <img
                  src={image}
                  alt={`Snack ${index + 1}`}
                  className="w-full h-auto block object-cover rounded-full shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                />
              </div>
            ))}
          </div>
        </section>

        {/* Bento Meals Section */}
        <section className="mb-20 md:mb-16 bg-black rounded-[20px] p-10 lg:p-8 md:p-6 sm:p-4">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-[18px] font-bold font-poppins text-shadow-golden sm:text-[20px] md:text-[24px] lg:text-[36px]">
              Bento Meals
            </h2>
            <button
              onClick={() => navigate('/order')}
              className="px-4 py-2 bg-gradient-golden text-black text-[12px] font-bold font-poppins rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:px-5 sm:py-2.5 sm:text-[14px] md:text-[15px] lg:px-6 lg:py-3 lg:text-[16px]"
            >
              Order Now
            </button>
          </div>
          <div className="w-full flex flex-row gap-[30px] overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px] hover:scrollbar-thumb-golden/80 md:gap-4 sm:gap-3">
            {bentoMeals.map((image, index) => (
              <div
                key={index}
                className="min-w-[130px] w-[130px] sm:min-w-[160px] sm:w-[160px] md:min-w-[180px] md:w-[180px] lg:min-w-[250px] lg:w-[250px] h-auto bg-transparent rounded-full overflow-visible flex-shrink-0 transition-all duration-300 hover:scale-110 cursor-pointer relative"
              >
                <img
                  src={image}
                  alt={`Bento Meal ${index + 1}`}
                  className="w-full h-auto block object-cover rounded-full shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                />
              </div>
            ))}
          </div>
        </section>

        {/* Ramen Section */}
        <section className="mb-12 bg-black rounded-[20px] p-10 lg:p-8 md:p-6 sm:p-4">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-[18px] font-bold font-poppins text-shadow-golden sm:text-[20px] md:text-[24px] lg:text-[36px]">
              Ramen
            </h2>
            <button
              onClick={() => navigate('/order')}
              className="px-4 py-2 bg-gradient-golden text-black text-[12px] font-bold font-poppins rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:px-5 sm:py-2.5 sm:text-[14px] md:text-[15px] lg:px-6 lg:py-3 lg:text-[16px]"
            >
              Order Now
            </button>
          </div>
          <div className="w-full flex flex-row gap-[30px] overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px] hover:scrollbar-thumb-golden/80 md:gap-4 sm:gap-3">
            {ramenBowls.map((image, index) => (
              <div
                key={index}
                className="min-w-[130px] w-[130px] sm:min-w-[160px] sm:w-[160px] md:min-w-[180px] md:w-[180px] lg:min-w-[250px] lg:w-[250px] h-auto bg-transparent rounded-full overflow-visible flex-shrink-0 transition-all duration-300 hover:scale-110 cursor-pointer relative"
              >
                <img
                  src={image}
                  alt={`Ramen ${index + 1}`}
                  className="w-full h-auto block object-cover rounded-full shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
                />
              </div>
            ))}
          </div>
        </section>
      </div>
    </div>
  );
};

export default Menu;

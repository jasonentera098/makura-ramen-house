import { useNavigate } from 'react-router-dom';

const Home = () => {
  const navigate = useNavigate();

  return (
    <div className="w-full h-auto relative bg-black overflow-x-hidden pt-0 pb-12 box-border gap-0 text-left text-[40px] text-gray-400 font-poppins">
      {/* Hero Section - Hidden on mobile (< 640px), shown on sm and above */}
      <div className="hidden sm:flex self-stretch flex-col items-start gap-px max-w-full mt-[100px]">
        <div className="self-stretch h-[420px] relative max-w-full overflow-hidden lg:h-[380px] md:h-[350px]">
          {/* Ramen Bowl Image */}
          <section className="absolute top-[45.3px] right-20 w-[450px] h-[372.3px] max-w-[55%] text-center text-[20px] text-white font-montserrat lg:right-10 lg:w-[350px] lg:h-[320px] md:right-5 md:w-[280px] md:h-[260px] md:top-[30px]">
            <img
              src="/public/Clip-path-group@2x.png"
              alt="Ramen Bowl"
              className="absolute top-0 left-0 w-full h-full object-contain z-10"
            />
            
            {/* Best Seller Badge */}
            <div className="absolute bottom-5 right-5 w-auto flex flex-row items-center justify-start p-3 box-border z-40 gap-4 rounded-[21px] bg-[#a0522d] md:p-2 md:gap-2">
              <div className="flex flex-col items-start relative z-50 gap-1">
                <b className="relative m-0 px-3 py-0.5 bg-golden text-black text-[13px] font-bold text-center whitespace-nowrap md:text-[11px] md:px-2">
                  Best Seller
                </b>
                <h3 className="m-0 relative text-[18px] font-bold font-inherit z-30 text-left whitespace-nowrap leading-tight md:text-[14px]">
                  TONKOTSU RAMEN
                </h3>
              </div>
              <div className="flex flex-col items-center justify-center p-0 m-0 relative text-[24px] text-black md:text-[20px]">
                <div className="flex items-center justify-center rounded-tr-[30px] rounded-bl-[30px] bg-[#d3d3d3] px-4 py-2 z-30 md:px-3 md:py-1.5">
                  <h3 className="relative m-0 text-[18px] leading-tight font-bold font-inherit z-10 text-center whitespace-nowrap md:text-[14px]">
                    139 Only!
                  </h3>
                </div>
              </div>
            </div>
          </section>

          {/* Hero Text */}
          <div className="absolute top-0 left-0 w-full flex flex-col items-start px-[150px] pr-[calc(45%+20px)] py-12 box-border gap-9 bg-gradient-golden lg:px-20 lg:pr-[40%] md:px-5 md:pr-[30%] md:gap-5 md:py-8">
            <div className="self-stretch flex flex-col items-start gap-6 max-w-full md:gap-4">
              <h1 className="m-0 relative text-black font-extrabold font-inherit z-10 lg:text-[32px] md:text-[20px]">
                WHERE EVERY BITE IS A JOURNEY.
              </h1>
              <div className="w-full max-w-[414px] relative text-[16px] font-semibold font-montserrat inline-block z-10 md:text-[14px] text-black">
                Serving delicious ramen and Japanese favorites made with fresh
                ingredients and authentic flavors
              </div>
            </div>
            <div className="w-auto flex justify-start p-0 box-border max-w-full">
              <button 
                onClick={() => navigate('/order')}
                className="cursor-pointer border-0 px-5 py-4 bg-black flex-1 rounded-[15px] z-10 hover:bg-[#2f4f4f] transition-colors md:px-4 md:py-3"
              >
                <div className="flex-1 relative text-[20px] font-extrabold font-poppins text-[#fffafa] text-center whitespace-nowrap z-20 md:text-[14px]">
                  ORDER NOW
                </div>
              </button>
            </div>
          </div>
        </div>

        {/* Best Selling Bowls Section - Desktop/Tablet */}
        <main className="self-stretch flex flex-col items-start px-0 pr-2.5 box-border gap-26 max-w-full lg:gap-13 md:gap-13">
          <section className="self-stretch flex flex-col items-end gap-10 max-w-full text-white md:gap-5">
            {/* Section Header */}
            <section className="self-stretch flex items-start justify-between gap-5 max-w-full text-center text-[40px] font-poppins flex-nowrap lg:text-[32px] md:text-[24px]">
              <h1 className="m-0 w-[441.2px] relative text-inherit font-bold font-inherit inline-block text-shadow-golden max-w-full">
                Best Selling Bowls
              </h1>
            </section>

            {/* Bowl Cards */}
            <section className="self-stretch flex justify-end px-16 pb-2.5 box-border max-w-full text-center text-[16px] text-white font-poppins lg:px-8 md:px-4 tablet-section-container">
              <div className="flex-1 flex flex-row justify-center gap-6 max-w-full lg:gap-5 md:gap-4 tablet-scroll-container horizontal-scroll-tablet">
                {/* Bowl Card 1 */}
                <div className="w-[240px] lg:w-[210px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(0,0,0,0.5)] cursor-pointer tablet-scroll-item">
                  <img
                    src="/public/pic@2x.png"
                    alt="Tonkotsu Ramen"
                    className="w-full h-[320px] lg:h-[280px] object-contain bg-[#d3d3d3] pt-4 px-3"
                  />
                  <div className="w-full bg-black px-4 py-4 mt-auto">
                    <b className="text-[13px] text-white block text-center lg:text-[12px]">TONKOTSU RAMEN</b>
                  </div>
                </div>

                {/* Bowl Card 2 */}
                <div className="w-[240px] lg:w-[210px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(0,0,0,0.5)] cursor-pointer tablet-scroll-item">
                  <img
                    src="/public/Group-38@2x.png"
                    alt="Katsu Curry Ramen"
                    className="w-full h-[320px] lg:h-[280px] object-contain bg-[#d3d3d3] pt-4 px-3"
                  />
                  <div className="w-full bg-black px-4 py-4 mt-auto">
                    <b className="text-[13px] text-white block text-center lg:text-[12px]">KATSU CURRY RAMEN</b>
                  </div>
                </div>

                {/* Bowl Card 3 */}
                <div className="w-[240px] lg:w-[210px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(0,0,0,0.5)] cursor-pointer tablet-scroll-item">
                  <img
                    src="/public/Untitled-design-2-2@2x.png"
                    alt="Cheesy Tonkotsu"
                    className="w-full h-[320px] lg:h-[280px] object-contain bg-[#d3d3d3] pt-4 px-3"
                  />
                  <div className="w-full bg-black px-4 py-4 mt-auto">
                    <b className="text-[13px] text-white block text-center lg:text-[12px]">CHEESY TONKOTSU</b>
                  </div>
                </div>

                {/* Bowl Card 4 */}
                <div className="w-[240px] lg:w-[210px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(0,0,0,0.5)] cursor-pointer tablet-scroll-item">
                  <img
                    src="/public/Untitled-design-3-1@2x.png"
                    alt="Red Ramen"
                    className="w-full h-[320px] lg:h-[280px] object-contain bg-[#d3d3d3] pt-4 px-3"
                  />
                  <div className="w-full bg-black px-4 py-4 mt-auto">
                    <b className="text-[13px] text-white block text-center lg:text-[12px]">RED RAMEN</b>
                  </div>
                </div>
              </div>
            </section>

            {/* Promo Section */}
            <section className="self-stretch flex items-start justify-between gap-5 max-w-full text-center text-[40px] font-poppins flex-nowrap lg:text-[32px] md:text-[24px]">
              <h2 className="m-0 w-[441.2px] relative text-inherit font-bold font-inherit inline-block text-shadow-golden max-w-full">
                Promo
              </h2>
            </section>

            <section className="self-stretch flex justify-end px-16 pb-2.5 box-border max-w-full text-center text-[16px] text-white font-poppins lg:px-8 md:px-4 tablet-section-container">
              <div className="flex-1 flex flex-row justify-center gap-11 max-w-full lg:gap-6 md:gap-5 tablet-scroll-container horizontal-scroll-tablet">
                <img
                  src="/public/unli1@2x.png"
                  alt="Promo 1"
                  className="relative object-cover h-[360px] lg:h-[310px] max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px] tablet-scroll-item"
                />
                <img
                  src="/public/unli2@2x.png"
                  alt="Promo 2"
                  className="relative object-cover h-[360px] lg:h-[310px] max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px] tablet-scroll-item"
                />
                <img
                  src="/public/648511596-1533822838745647-5485652144745721927-n-1@2x.png"
                  alt="Promo 3"
                  className="relative object-cover h-[360px] lg:h-[310px] max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px] tablet-scroll-item"
                />
                <img
                  src="/public/561191296-1404399051688027-8262375691566810025-n-2@2x.png"
                  alt="Promo 4"
                  className="relative object-cover h-[360px] lg:h-[310px] max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px] tablet-scroll-item"
                />
              </div>
            </section>
          </section>

          {/* Barkada Feast Section */}
          <section className="self-stretch flex flex-col items-end gap-10 max-w-full text-white md:gap-5">
            {/* Section Header */}
            <section className="self-stretch flex items-start justify-between gap-5 max-w-full text-center text-[40px] font-poppins flex-nowrap lg:text-[32px] md:text-[24px]">
              <h2 className="m-0 w-[441.2px] relative text-inherit font-bold font-inherit inline-block text-shadow-golden max-w-full">
                Barkada Feast
              </h2>
            </section>

            {/* Barkada Feast Images */}
            <section className="self-stretch flex justify-end px-16 pb-32 box-border max-w-full text-center text-[16px] text-white font-poppins lg:px-8 lg:pb-21 md:px-4 md:pb-21">
              <div className="flex-1 flex flex-row justify-center gap-10 max-w-full lg:gap-16 md:gap-8">
                <img
                  src="/public/image-2@2x.png"
                  alt="Barkada Feast 1"
                  className="relative object-cover h-[350px] w-auto max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px]"
                />
                <img
                  src="/public/image-31@2x.png"
                  alt="Barkada Feast 2"
                  className="relative object-cover h-[350px] w-auto max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px]"
                />
                <img
                  src="/public/image-1@2x.png"
                  alt="Barkada Feast 3"
                  className="relative object-cover h-[350px] w-auto max-w-full transition-all duration-300 hover:-translate-y-2.5 hover:scale-105 hover:shadow-[0_15px_30px_rgba(242,224,116,0.4)] cursor-pointer rounded-[10px]"
                />
              </div>
            </section>
          </section>
        </main>
      </div>

      {/* Mobile-Only Content (< 640px) - Horizontally Scrollable Sections */}
      <div className="block sm:hidden mt-[80px] px-4">

        {/* Mobile Hero Section */}
        <div className="relative w-full mb-10 rounded-[20px] overflow-hidden bg-gradient-golden">
          {/* Hero Text only - no image on mobile */}
          <div className="px-6 py-10 flex flex-col gap-4">
            <h1 className="m-0 text-black text-[22px] font-extrabold font-poppins leading-tight">
              WHERE EVERY BITE IS A JOURNEY.
            </h1>
            <p className="text-black text-[13px] font-semibold font-montserrat leading-snug">
              Serving delicious ramen and Japanese favorites made with fresh ingredients and authentic flavors
            </p>
            <button 
              onClick={() => navigate('/order')}
              className="cursor-pointer border-0 px-5 py-3 bg-black rounded-[15px] w-fit self-start hover:bg-[#2f4f4f] transition-colors"
            >
              <span className="text-[14px] font-extrabold font-poppins text-white">ORDER NOW</span>
            </button>
          </div>
        </div>
        {/* Best Selling Bowls Section - Mobile */}
        <section className="mb-16">
          <h1 className="text-[18px] font-bold font-poppins text-white text-shadow-golden mb-6 text-center">
            Best Selling Bowls
          </h1>
          <div className="w-full flex flex-row gap-4 overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px]">
            {/* Bowl Card 1 */}
            <div className="min-w-[180px] w-[180px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105">
              <img
                src="/public/pic@2x.png"
                alt="Tonkotsu Ramen"
                className="w-full h-[160px] object-contain bg-[#d3d3d3] pt-3 px-2"
              />
              <div className="w-full bg-black px-3 py-3 mt-auto">
                <b className="text-[11px] text-white block text-center">TONKOTSU RAMEN</b>
              </div>
            </div>

            {/* Bowl Card 2 */}
            <div className="min-w-[180px] w-[180px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105">
              <img
                src="/public/Group-38@2x.png"
                alt="Katsu Curry Ramen"
                className="w-full h-[160px] object-contain bg-[#d3d3d3] pt-3 px-2"
              />
              <div className="w-full bg-black px-3 py-3 mt-auto">
                <b className="text-[11px] text-white block text-center">KATSU CURRY RAMEN</b>
              </div>
            </div>

            {/* Bowl Card 3 */}
            <div className="min-w-[180px] w-[180px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105">
              <img
                src="/public/Untitled-design-2-2@2x.png"
                alt="Cheesy Tonkotsu"
                className="w-full h-[160px] object-contain bg-[#d3d3d3] pt-3 px-2"
              />
              <div className="w-full bg-black px-3 py-3 mt-auto">
                <b className="text-[11px] text-white block text-center">CHEESY TONKOTSU</b>
              </div>
            </div>

            {/* Bowl Card 4 */}
            <div className="min-w-[180px] w-[180px] flex-shrink-0 rounded-[24px] bg-[#d3d3d3] flex flex-col items-center overflow-hidden cursor-pointer transition-all duration-300 hover:scale-105">
              <img
                src="/public/Untitled-design-3-1@2x.png"
                alt="Red Ramen"
                className="w-full h-[160px] object-contain bg-[#d3d3d3] pt-3 px-2"
              />
              <div className="w-full bg-black px-3 py-3 mt-auto">
                <b className="text-[11px] text-white block text-center">RED RAMEN</b>
              </div>
            </div>
          </div>
        </section>

        {/* Promo Section - Mobile */}
        <section className="mb-16">
          <h2 className="text-[18px] font-bold font-poppins text-white text-shadow-golden mb-6 text-center">
            Promo
          </h2>
          <div className="w-full flex flex-row gap-4 overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px]">
            <img
              src="/public/unli1@2x.png"
              alt="Promo 1"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)] bg-white"
            />
            <img
              src="/public/unli2@2x.png"
              alt="Promo 2"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)] bg-white"
            />
            <img
              src="/public/648511596-1533822838745647-5485652144745721927-n-1@2x.png"
              alt="Promo 3"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)] bg-white"
            />
            <img
              src="/public/561191296-1404399051688027-8262375691566810025-n-2@2x.png"
              alt="Promo 4"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)] bg-white"
            />
          </div>
        </section>

        {/* Barkada Feast Section - Mobile */}
        <section className="mb-12">
          <h2 className="text-[18px] font-bold font-poppins text-white text-shadow-golden mb-6 text-center">
            Barkada Feast
          </h2>
          <div className="w-full flex flex-row gap-4 overflow-x-auto overflow-y-hidden pb-5 scroll-smooth scrollbar-thin scrollbar-track-golden/10 scrollbar-thumb-golden scrollbar-thumb-rounded-[10px]">
            <img
              src="/public/image-2@2x.png"
              alt="Barkada Feast 1"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
            />
            <img
              src="/public/image-31@2x.png"
              alt="Barkada Feast 2"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
            />
            <img
              src="/public/image-1@2x.png"
              alt="Barkada Feast 3"
              className="min-w-[220px] w-[220px] h-auto object-contain flex-shrink-0 transition-all duration-300 hover:scale-105 cursor-pointer rounded-[10px] shadow-[0_8px_25px_rgba(242,224,116,0.3)]"
            />
          </div>
        </section>
      </div>
    </div>
  );
};

export default Home;

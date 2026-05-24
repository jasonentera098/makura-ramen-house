const About = () => {
  return (
    <div className="w-full min-h-screen bg-black text-white pt-[100px] pb-12">
      <div className="max-w-[1440px] mx-auto px-8 lg:px-6 md:px-4">
        {/* Page Title */}
        <div className="text-center mb-16 md:mb-12">
          <h1 className="pt-[40px] text-[22px] font-bold font-poppins text-shadow-golden mb-4 sm:text-[28px] md:text-[32px] lg:text-[40px]">
            About Makura Ramen House
          </h1>
          <p className="text-[13px] font-montserrat text-gray-300 max-w-[800px] mx-auto sm:text-[14px] md:text-[16px] lg:text-[18px]">
            Discover the story behind our passion for authentic Japanese ramen
          </p>
        </div>

        {/* Our Story Section */}
        <section className="mb-20 md:mb-16">
          <div className="bg-gradient-golden rounded-[20px] p-12 lg:p-10 md:p-8 sm:p-6">
            <h2 className="text-[22px] font-bold font-poppins text-black mb-6 sm:text-[24px] md:text-[28px] lg:text-[36px]">
              Our Story
            </h2>
            <div className="text-[13px] font-montserrat text-black leading-relaxed space-y-4 sm:text-[15px] md:text-[15px] lg:text-[18px]">
              <p>
                Makura Ramen House was born from a deep love for authentic Japanese cuisine and a desire to bring the rich, comforting flavors of traditional ramen to Gingoog City. Our journey began with a simple dream: to create a place where every bowl tells a story and every bite takes you on a culinary journey to Japan.
              </p>
              <p>
                We believe that great ramen starts with quality ingredients and time-honored techniques. Our chefs have trained extensively to master the art of ramen-making, from preparing the perfect broth that simmers for hours to hand-pulling our noodles with precision and care.
              </p>
              <p>
                At Makura Ramen House, we're not just serving food – we're creating experiences. Whether you're enjoying a quiet meal alone or celebrating with friends and family, we want every visit to be memorable.
              </p>
            </div>
          </div>
        </section>

        {/* Vision & Mission Section */}
        <section className="mb-12 md:mb-16">
          <h2 className="text-[18px] font-bold font-poppins text-center text-shadow-golden mb-6 sm:text-[22px] md:text-[28px] lg:text-[32px]">
            Vision & Mission
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6 lg:gap-6">
            {/* Vision Card */}
            <div className="relative rounded-[20px] overflow-hidden group cursor-pointer transition-all duration-300 hover:-translate-y-2 hover:shadow-[0_15px_40px_rgba(242,224,116,0.4)]">
              <img
                src="/public/pic@2x.png"
                alt="Vision"
                className="w-full h-[220px] object-cover sm:h-[280px] md:h-[300px] lg:h-[350px]"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/50 to-transparent flex flex-col justify-end p-5 md:p-6 lg:p-8">
                <h3 className="text-[16px] font-bold font-poppins text-golden mb-2 sm:text-[20px] md:text-[22px] lg:text-[24px]">
                  Our Vision
                </h3>
                <p className="text-[12px] font-montserrat text-white leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[16px]">
                  To become the premier destination for authentic Japanese ramen in the region, known for our commitment to quality, authenticity, and exceptional dining experiences.
                </p>
              </div>
            </div>

            {/* Mission Card */}
            <div className="relative rounded-[20px] overflow-hidden group cursor-pointer transition-all duration-300 hover:-translate-y-2 hover:shadow-[0_15px_40px_rgba(242,224,116,0.4)]">
              <img
                src="/public/Group-38@2x.png"
                alt="Mission"
                className="w-full h-[220px] object-cover sm:h-[280px] md:h-[300px] lg:h-[350px]"
              />
              <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/50 to-transparent flex flex-col justify-end p-5 md:p-6 lg:p-8">
                <h3 className="text-[16px] font-bold font-poppins text-golden mb-2 sm:text-[20px] md:text-[22px] lg:text-[24px]">
                  Our Mission
                </h3>
                <p className="text-[12px] font-montserrat text-white leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[16px]">
                  To serve delicious, authentic ramen made with fresh ingredients and traditional techniques, while creating a warm and welcoming atmosphere for all our guests.
                </p>
              </div>
            </div>
          </div>
        </section>

        {/* What Makes Our Ramen Unique Section */}
        <section className="mb-12">
          <h2 className="text-[18px] font-bold font-poppins text-center text-shadow-golden mb-6 sm:text-[22px] md:text-[28px] lg:text-[32px]">
            What Makes Our Ramen Unique
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 lg:gap-6">
            {/* Feature 1 */}
            <div className="bg-white/5 rounded-[20px] p-5 backdrop-blur-sm border border-golden/20 transition-all duration-300 hover:bg-white/10 hover:border-golden/40 sm:p-6 lg:p-6">
              <div className="text-[32px] mb-3 sm:text-[40px] lg:text-[48px]">🍜</div>
              <h3 className="text-[15px] font-bold font-poppins text-golden mb-2 sm:text-[18px] md:text-[20px] lg:text-[22px]">
                Authentic Broth
              </h3>
              <p className="text-[12px] font-montserrat text-gray-300 leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[15px]">
                Our broths are simmered for over 12 hours using traditional methods, creating rich, complex flavors that are the foundation of every bowl.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="bg-white/5 rounded-[20px] p-5 backdrop-blur-sm border border-golden/20 transition-all duration-300 hover:bg-white/10 hover:border-golden/40 sm:p-6 lg:p-6">
              <div className="text-[32px] mb-3 sm:text-[40px] lg:text-[48px]">🥢</div>
              <h3 className="text-[15px] font-bold font-poppins text-golden mb-2 sm:text-[18px] md:text-[20px] lg:text-[22px]">
                Fresh Ingredients
              </h3>
              <p className="text-[12px] font-montserrat text-gray-300 leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[15px]">
                We source the freshest local and imported ingredients daily, ensuring every bowl is made with the highest quality components.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="bg-white/5 rounded-[20px] p-5 backdrop-blur-sm border border-golden/20 transition-all duration-300 hover:bg-white/10 hover:border-golden/40 sm:p-6 lg:p-6">
              <div className="text-[32px] mb-3 sm:text-[40px] lg:text-[48px]">👨‍🍳</div>
              <h3 className="text-[15px] font-bold font-poppins text-golden mb-2 sm:text-[18px] md:text-[20px] lg:text-[22px]">
                Expert Craftsmanship
              </h3>
              <p className="text-[12px] font-montserrat text-gray-300 leading-relaxed sm:text-[13px] md:text-[14px] lg:text-[15px]">
                Our chefs are trained in traditional Japanese ramen-making techniques, bringing years of experience to every bowl they create.
              </p>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
};

export default About;

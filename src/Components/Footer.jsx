const Footer = () => {
  return (
    <footer className="footer self-stretch flex items-center justify-center py-6 px-6 box-border text-center text-black font-poppins max-w-full bg-gradient-golden sm:py-8 sm:px-10 lg:py-8 lg:px-16">
      <div className="w-full max-w-[1288px] flex items-center justify-center">
        {/* Three columns: always row on sm+, stacked on mobile */}
        <div className="w-full flex flex-col items-center gap-6 sm:flex-row sm:items-start sm:justify-between sm:gap-8">

          {/* LEFT — Business Info */}
          <div className="flex flex-col items-center gap-3 sm:items-start sm:flex-1">
            <h2 className="m-0 text-[14px] font-bold font-poppins text-black sm:text-[16px] lg:text-[18px]">
              Business Info
            </h2>
            <p className="m-0 text-[12px] font-medium font-montserrat text-black leading-relaxed sm:text-[13px] lg:text-[14px] sm:text-left">
              Makura Ramen House<br />
              Gingoog City, Misamis Oriental<br />
              Contact: 09534879391
            </p>
          </div>

          {/* CENTER — Business Hours */}
          <div className="flex flex-col items-center gap-3 sm:items-center sm:flex-1">
            <h2 className="m-0 text-[14px] font-bold font-poppins text-black sm:text-[16px] lg:text-[18px]">
              Business Hours
            </h2>
            <p className="m-0 text-[12px] font-medium font-montserrat text-black leading-relaxed sm:text-[13px] lg:text-[14px]">
              Mon - Fri: 11am - 10pm<br />
              Sat - Sun: 11am - 12am
            </p>
          </div>

          {/* RIGHT — Follow Us on Facebook */}
          <div className="flex flex-col items-center gap-3 sm:items-end sm:flex-1">
            <h2 className="m-0 text-[14px] font-bold font-poppins text-black sm:text-[16px] lg:text-[18px]">
              Follow Us on Facebook
            </h2>
            <div className="flex items-center gap-2">
              <span className="text-[12px] font-medium font-montserrat text-black sm:text-[13px] lg:text-[14px]">
                Makura Ramen House
              </span>
              <img
                src="/public/image-5@2x.png"
                alt="Facebook"
                className="w-6 h-6 sm:w-8 sm:h-8 object-cover"
              />
            </div>
          </div>

        </div>
      </div>
    </footer>
  );
};

export default Footer;

import { useState } from 'react';

const Contact = () => {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    subject: '',
    message: '',
  });

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log('Form submitted:', formData);
  };

  return (
    <div className="w-full min-h-screen bg-black text-white pt-[100px] pb-12">
      <div className="max-w-[1440px] mx-auto px-4 sm:px-6 md:px-6 lg:px-6">

        {/* Hero Section */}
        <div className="relative w-full h-[200px] mb-10 rounded-[20px] overflow-hidden sm:h-[250px] md:h-[300px] lg:h-[350px]">
          <img
            src="/public/Clip-path-group@2x.png"
            alt="Contact Hero"
            className="w-full h-full object-cover"
          />
          <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/50 to-transparent flex flex-col justify-end p-5 sm:p-6 md:p-8 lg:p-10">
            <h1 className="text-[22px] font-bold font-poppins text-golden text-shadow-golden mb-2 sm:text-[28px] md:text-[32px] lg:text-[40px]">
              Get In Touch
            </h1>
            <p className="text-[12px] font-montserrat text-white max-w-[600px] sm:text-[14px] md:text-[16px] lg:text-[18px]">
              We'd love to hear from you! Reach out to us for reservations, inquiries, or feedback.
            </p>
          </div>
        </div>

        {/* Contact Form Section */}
        <section className="mb-12 md:mb-16">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 md:gap-8 lg:gap-10">

            {/* Contact Information */}
            <div className="flex flex-col gap-5">
              <h2 className="text-[18px] font-bold font-poppins text-golden text-shadow-golden sm:text-[22px] md:text-[28px] lg:text-[32px]">
                Contact Information
              </h2>

              <div className="flex flex-col gap-4">
                {/* Address */}
                <div className="flex items-start gap-3">
                  <div className="text-[22px] text-golden sm:text-[28px] md:text-[32px]">📍</div>
                  <div>
                    <h3 className="text-[14px] font-bold font-poppins text-white mb-1 sm:text-[16px] md:text-[18px] lg:text-[20px]">
                      Address
                    </h3>
                    <p className="text-[12px] font-montserrat text-gray-300 sm:text-[13px] md:text-[15px] lg:text-[16px]">
                      Makura Ramen House<br />
                      Gingoog City, Misamis Oriental
                    </p>
                  </div>
                </div>

                {/* Phone */}
                <div className="flex items-start gap-3">
                  <div className="text-[22px] text-golden sm:text-[28px] md:text-[32px]">📞</div>
                  <div>
                    <h3 className="text-[14px] font-bold font-poppins text-white mb-1 sm:text-[16px] md:text-[18px] lg:text-[20px]">
                      Phone
                    </h3>
                    <p className="text-[12px] font-montserrat text-gray-300 sm:text-[13px] md:text-[15px] lg:text-[16px]">
                      09534879391
                    </p>
                  </div>
                </div>

                {/* Email */}
                <div className="flex items-start gap-3">
                  <div className="text-[22px] text-golden sm:text-[28px] md:text-[32px]">✉️</div>
                  <div>
                    <h3 className="text-[14px] font-bold font-poppins text-white mb-1 sm:text-[16px] md:text-[18px] lg:text-[20px]">
                      Email
                    </h3>
                    <p className="text-[12px] font-montserrat text-gray-300 sm:text-[13px] md:text-[15px] lg:text-[16px]">
                      info@makuraramen.com
                    </p>
                  </div>
                </div>

                {/* Business Hours */}
                <div className="flex items-start gap-3">
                  <div className="text-[22px] text-golden sm:text-[28px] md:text-[32px]">🕐</div>
                  <div>
                    <h3 className="text-[14px] font-bold font-poppins text-white mb-1 sm:text-[16px] md:text-[18px] lg:text-[20px]">
                      Business Hours
                    </h3>
                    <p className="text-[12px] font-montserrat text-gray-300 sm:text-[13px] md:text-[15px] lg:text-[16px]">
                      Mon - Fri: 11am - 10pm<br />
                      Sat - Sun: 11am - 12am
                    </p>
                  </div>
                </div>
              </div>
            </div>

            {/* Contact Form */}
            <div className="bg-white/5 rounded-[20px] p-5 backdrop-blur-sm border border-golden/20 sm:p-6 lg:p-6">
              <h2 className="text-[16px] font-bold font-poppins text-golden mb-4 sm:text-[20px] md:text-[22px] lg:text-[24px]">
                Send Us a Message
              </h2>
              <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                <div className="grid grid-cols-2 gap-3 md:gap-6">
                  <div>
                    <label className="block text-[12px] font-montserrat text-white mb-1.5 sm:text-[14px] md:text-[15px] lg:text-[16px]">
                      First Name
                    </label>
                    <input
                      type="text"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleChange}
                      className="w-full px-3 py-2 bg-black/50 border border-golden/30 rounded-[10px] text-white text-[12px] font-montserrat focus:outline-none focus:border-golden transition-colors sm:text-[14px] sm:px-4 sm:py-3"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-[12px] font-montserrat text-white mb-1.5 sm:text-[14px] md:text-[15px] lg:text-[16px]">
                      Last Name
                    </label>
                    <input
                      type="text"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleChange}
                      className="w-full px-3 py-2 bg-black/50 border border-golden/30 rounded-[10px] text-white text-[12px] font-montserrat focus:outline-none focus:border-golden transition-colors sm:text-[14px] sm:px-4 sm:py-3"
                      required
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-[12px] font-montserrat text-white mb-1.5 sm:text-[14px] md:text-[15px] lg:text-[16px]">
                    Email
                  </label>
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    className="w-full px-3 py-2 bg-black/50 border border-golden/30 rounded-[10px] text-white text-[12px] font-montserrat focus:outline-none focus:border-golden transition-colors sm:text-[14px] sm:px-4 sm:py-3"
                    required
                  />
                </div>

                <div>
                  <label className="block text-[12px] font-montserrat text-white mb-1.5 sm:text-[14px] md:text-[15px] lg:text-[16px]">
                    Subject
                  </label>
                  <input
                    type="text"
                    name="subject"
                    value={formData.subject}
                    onChange={handleChange}
                    className="w-full px-3 py-2 bg-black/50 border border-golden/30 rounded-[10px] text-white text-[12px] font-montserrat focus:outline-none focus:border-golden transition-colors sm:text-[14px] sm:px-4 sm:py-3"
                    required
                  />
                </div>

                <div>
                  <label className="block text-[12px] font-montserrat text-white mb-1.5 sm:text-[14px] md:text-[15px] lg:text-[16px]">
                    Message
                  </label>
                  <textarea
                    name="message"
                    value={formData.message}
                    onChange={handleChange}
                    rows="4"
                    className="w-full px-3 py-2 bg-black/50 border border-golden/30 rounded-[10px] text-white text-[12px] font-montserrat focus:outline-none focus:border-golden transition-colors resize-none sm:text-[14px] sm:px-4 sm:py-3"
                    required
                  ></textarea>
                </div>

                <button
                  type="submit"
                  className="w-full px-4 py-2.5 bg-gradient-golden text-black text-[13px] font-bold font-poppins border-none rounded-[10px] cursor-pointer transition-all duration-300 hover:-translate-y-0.5 hover:shadow-[0_8px_20px_rgba(242,224,116,0.5)] sm:text-[15px] sm:py-3 md:text-[16px] lg:text-[18px] lg:px-8 lg:py-4"
                >
                  Send Message
                </button>
              </form>
            </div>
          </div>
        </section>

        {/* Map Section */}
        <section className="mb-12">
          <h2 className="text-[18px] font-bold font-poppins text-golden text-shadow-golden mb-5 text-center sm:text-[22px] md:text-[28px] lg:text-[32px]">
            Visit Us Here
          </h2>
          <div className="w-full h-[250px] rounded-[20px] overflow-hidden border-2 border-golden/30 sm:h-[300px] md:h-[350px] lg:h-[400px]">
            <iframe
              src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3942.5308237078034!2d125.09700477479137!3d8.830069191223563!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x33002f9057aeb4f3%3A0x7001ede9cdcc66da!2sMakura!5e0!3m2!1sen!2sph!4v1777750098773!5m2!1sen!2sph"
              width="100%"
              height="100%"
              style={{ border: 0 }}
              allowFullScreen=""
              loading="lazy"
              referrerPolicy="no-referrer-when-downgrade"
              title="Makura Ramen House Location"
            ></iframe>
          </div>
        </section>
      </div>
    </div>
  );
};

export default Contact;

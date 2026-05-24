// Shared Navigation JavaScript for all pages

// Scroll effect for header transparency
window.addEventListener('scroll', function() {
  var header = document.querySelector('.header');
  if (header && window.scrollY > 50) {
    header.classList.add('scrolled');
  } else if (header) {
    header.classList.remove('scrolled');
  }
});

// Burger Menu Toggle
var burgerMenu = document.getElementById("burgerMenu");
var mobileMenu = document.getElementById("mobileMenu");

if (burgerMenu && mobileMenu) {
  burgerMenu.addEventListener("click", function() {
    mobileMenu.classList.toggle("active");
    burgerMenu.classList.toggle("active");
  });
}

// Desktop Navigation
var homeText = document.getElementById("homeText");
if (homeText) {
  homeText.addEventListener("click", function (e) {
    window.location.href = "LandPage.html";
  });
}

var aboutText = document.getElementById("aboutText");
if (aboutText) {
  aboutText.addEventListener("click", function (e) {
    window.location.href = "about.html";
  });
}

var menuText = document.getElementById("menuText");
if (menuText) {
  menuText.addEventListener("click", function (e) {
    window.location.href = "menu.html";
  });
}

var promoText = document.getElementById("promoText");
if (promoText) {
  promoText.addEventListener("click", function (e) {
    window.location.href = "promo.html";
  });
}

var contactsText = document.getElementById("contactsText");
if (contactsText) {
  contactsText.addEventListener("click", function (e) {
    window.location.href = "contact.html";
  });
}

var groupButton = document.getElementById("groupButton");
if (groupButton) {
  groupButton.addEventListener("click", function (e) {
    window.location.href = "login.html";
  });
}

// Mobile Navigation
var homeTextMobile = document.getElementById("homeTextMobile");
if (homeTextMobile) {
  homeTextMobile.addEventListener("click", function (e) {
    window.location.href = "LandPage.html";
  });
}

var aboutTextMobile = document.getElementById("aboutTextMobile");
if (aboutTextMobile) {
  aboutTextMobile.addEventListener("click", function (e) {
    window.location.href = "about.html";
  });
}

var menuTextMobile = document.getElementById("menuTextMobile");
if (menuTextMobile) {
  menuTextMobile.addEventListener("click", function (e) {
    window.location.href = "menu.html";
  });
}

var promoTextMobile = document.getElementById("promoTextMobile");
if (promoTextMobile) {
  promoTextMobile.addEventListener("click", function (e) {
    window.location.href = "promo.html";
  });
}

var contactsTextMobile = document.getElementById("contactsTextMobile");
if (contactsTextMobile) {
  contactsTextMobile.addEventListener("click", function (e) {
    window.location.href = "contact.html";
  });
}

var loginBtnMobile = document.getElementById("loginBtnMobile");
if (loginBtnMobile) {
  loginBtnMobile.addEventListener("click", function (e) {
    window.location.href = "login.html";
  });
}

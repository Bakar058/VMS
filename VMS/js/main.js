$(function() {
    
    "use strict";
    
    //===== Prealoder
    
    $(window).on('load', function(event) {
        $(".preloader").delay(500).fadeOut(500);
    });
    
    
    //===== Mobile Menu 
    
    $(".navbar-toggler").on('click', function() {
        $(this).toggleClass("active");
    });
    
    
    var subMenu = $(".sub-menu-bar .navbar-nav .sub-menu");
    
    if(subMenu.length) {
        subMenu.parent('li').children('a').append(function () {
            return '<button class="sub-nav-toggler"> <i class="fa fa-chevron-down"></i> </button>';
        });
        
        var subMenuToggler = $(".sub-menu-bar .navbar-nav .sub-nav-toggler");
        
        subMenuToggler.on('click', function() {
            $(this).parent().parent().children(".sub-menu").slideToggle();
            return false
        });
        
    }
    
    
    //===== Sticky
    
    $(window).on('scroll', function(event) {    
        var scroll = $(window).scrollTop();
        if (scroll < 245) {
            $(".navigation").removeClass("sticky");
            $(".header-2 img").attr("src", "images/logo-light.png");
        } else{
            $(".navigation").addClass("sticky");
            $(".header-2 img").attr("src", "images/logo.png");
        }
    });
    
    //===== Account Popup
    
    $(".popup-close").on("click", function(){
        $(".account-popup-area").fadeOut(500);
    });
    
    
    //===== Slider slick
    
    function mainSlider() {
        var BasicSlider = $('.slider-active');
        var BasicSliderTow = $('.content-slied');
        
        BasicSlider.on('init', function(e, slick) {
            var $firstAnimatingElements = $('.single-slider:first-child').find('[data-animation]');
            doAnimations($firstAnimatingElements);
        });
        BasicSliderTow.on('init', function(e, slick) {
            var $firstAnimatingElements = $('.single-slider:first-child').find('[data-animation]');
            doAnimations($firstAnimatingElements);
        });
        
        
        BasicSlider.on('beforeChange', function(e, slick, currentSlide, nextSlide) {
            var $animatingElements = $('.single-slider[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
            doAnimations($animatingElements);
        });
        BasicSliderTow.on('beforeChange', function(e, slick, currentSlide, nextSlide) {
            var $animatingElements = $('.single-slider[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
            doAnimations($animatingElements);
        });
        
        
        BasicSlider.slick({
            autoplay: true,
            autoplaySpeed: 6000,
            dots: false,
            fade: true,
			arrows: true,
            prevArrow:'<span class="prev"><i class="fa fa-angle-left"></i></span>',
            nextArrow: '<span class="next"><i class="fa fa-angle-right"></i></span>',
            responsive: [
                { breakpoint: 767, settings: { dots: false, arrows: false } }
            ]
        });
        
        BasicSliderTow.slick({
            autoplay: true,
            autoplaySpeed: 6000,
            dots: false,
            fade: true,
			arrows: true,
            prevArrow:'<span class="prev"><i class="fa fa-angle-left"></i></span>',
            nextArrow: '<span class="next"><i class="fa fa-angle-right"></i></span>',
            responsive: [
                { breakpoint: 767, settings: { dots: false, arrows: false } }
            ]
        });

        function doAnimations(elements) {
            var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
            elements.each(function() {
                var $this = $(this);
                var $animationDelay = $this.data('delay');
                var $animationType = 'animated ' + $this.data('animation');
                $this.css({
                    'animation-delay': $animationDelay,
                    '-webkit-animation-delay': $animationDelay
                });
                $this.addClass($animationType).one(animationEndEvents, function() {
                    $this.removeClass($animationType);
                });
            });
        }
    }
    mainSlider();
    
    
    
    //===== Search Box
    
    $("#user").on('click', function(){
        $(".user-box").fadeToggle();
    });
    
    
    //===== Magnific Popup
    
    $(".videi-popup").magnificPopup({
      type: 'iframe',
    });
    
    
    //===== Owl Carousel Trusted Slider
    
    $(".trusted-slied").owlCarousel({
        loop:true,
        nav:false,
        dots:true,
        autoplay:true,
        smartSpeed:800,
        items:1,
    })
    
    
    //===== Owl Carousel Client Slied
    
    $(".client-slied").owlCarousel({
        loop:true,
        nav:false,
        dots:false,
        autoplay:true,
        smartSpeed:800,
        responsive:{
            0:{
                items:1
            },
            768:{
                items:2
            },
            992:{
                items:2
            }
        }
    })
    
    
    //===== Owl Carousel Blog Slied
    
    $(".blog-slied").owlCarousel({
        loop:true,
        nav:false,
        dots:true,
        autoplay:true,
        smartSpeed:800,
        responsive:{
            0:{
                items:1,
                dots:false,
            },
            768:{
                items:2
            },
            992:{
                items:3
            }
        }
    })

    
    //===== Owl Carousel Brand Slied
    
    $(".brand-slied").owlCarousel({
        loop:true,
        nav:false,
        dots:false,
        autoplay:true,
        smartSpeed:800,
        responsive:{
            0:{
                items:2,
            },
            576:{
                items:3
            },
            768:{
                items:4
            },
            992:{
                items:6
            }
        }
    })
    
    
    //===== Back to top
    
    // Show or hide the sticky footer button
    $(window).on('scroll', function(event) {
        if($(this).scrollTop() > 600){
            $(".back-to-top").fadeIn(200)
        } else{
            $(".back-to-top").fadeOut(200)
        }
    });
    
    
    //Animate the scroll to yop
    $(".back-to-top").on('click', function(event) {
        event.preventDefault();
        
        $("html, body").animate({
            scrollTop: 0,
        }, 1500);
    });
    
    
    //===== Owl Carousel Products Slied
    
    $(".products-slied").owlCarousel({
        loop:true,
        nav:false,
        dots:false,
        autoplay:true,
        smartSpeed:800,
        responsive:{
            0:{
                items:1,
                dots:false,
            },
            576:{
                items:2
            },
            768:{
                items:2
            },
            992:{
                items:4
            }
        }
    })
    
    
    //===== slick Products View
    
     $(".products-viwe").slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        fade: true,
        asNavFor: '.products-thum'
    });
    
    $(".products-thum").slick({
        slidesToShow: 4,
        slidesToScroll: 1,
        asNavFor: '.products-viwe',
        dots: false,
        infinite: false,
        arrows: true,
        prevArrow: '<i class="fa fa-angle-left"></i>',
        nextArrow: '<i class="fa fa-angle-right"></i>',
        centerMode: false,
        focusOnSelect: true
    });
    
    
    //===== Quantite
    
    $('input[type="number"]').niceNumber();

    
    //===== Nice Select
    
    $("select").niceSelect();
    
    
    //===== Validator
    
    $("#contact-form").validator();
    

    
});
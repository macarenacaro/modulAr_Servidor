document.addEventListener("DOMContentLoaded", function () {
    // Selecciona los elementos por sus IDs
    var elementos = [
        { boton: document.getElementById("notificaciones"), collapsible: document.getElementById("notificationsColl"), contador: 0 },
        { boton: document.getElementById("mensajes"), collapsible: document.getElementById("mensajesColl"), contador: 0 },
        { boton: document.getElementById("compras"), collapsible: document.getElementById("shopingColl"), contador: 0 },
        { boton: document.getElementById("login"), collapsible: document.getElementById("loginUserColl"), contador: 0 },
        { boton: document.getElementById("portrait"), collapsible: document.getElementById("perfilColl"), contador: 0 },
        { boton: document.getElementById("boton"), collapsible: document.getElementById("menuColl"), contador: 0 },
    ];

    ocultarTodosLosElementos();

    // Función para ocultar todos los elementos
    function ocultarTodosLosElementos() {
        elementos.forEach(function (elemento) {
            elemento.collapsible.style.display = 'none';
        });
    }

    // Agrega eventos de clic a los elementos
    elementos.forEach(function (elemento) {
        elemento.boton.addEventListener("click", function () {
            ocultarTodosLosElementos();
            elemento.collapsible.style.display = 'block';
            elemento.contador++;

            if (elemento.contador == 2) {
                elemento.collapsible.style.display = 'none';
                elemento.contador = 0;
            }
        });
    });
});






/*evento carrusel*/

var documentElement = document;

documentElement.addEventListener('DOMContentLoaded', function () {

    const sliderMe = () => {
        let currentPosition = 0,
            sliderItem = documentElement.querySelectorAll('.slider-item'),
            sliderItemWidth = window.getComputedStyle(sliderItem[0]).flexBasis.match(/\d+\.?\d+/g),
            sliderInner = documentElement.querySelector('.slider-inner'),

            control = {
                next: documentElement.querySelector('#next'),
                slideNext() {
                    currentPosition += parseFloat(sliderItemWidth);
                    if (currentPosition > limitPosition) {
                        currentPosition = 0;
                    }
                    sliderInner.style.right = currentPosition + '%';
                },
                prev: documentElement.querySelector('#prev'),

                slidePrev() {
                    currentPosition -= parseFloat(sliderItemWidth);
                    if (currentPosition < 0) {
                        currentPosition = limitPosition;
                    }
                    sliderInner.style.right = currentPosition + '%';
                }
            },
            limitPosition = sliderItemWidth * (sliderItem.length - Math.floor(200 / sliderItemWidth));

        control.next.addEventListener('click', control.slideNext)
        control.prev.addEventListener('click', control.slidePrev)

        window.addEventListener("resize", function () {
            currentPosition = 0;
            documentElement.querySelector('.slider-inner').style.right = currentPosition + '%';
        })
    }
    sliderMe();

    window.addEventListener("resize", sliderMe)

});




//*PARA SELECCIONAR LOS DIV DEL CARROUSEL!*//
var disgraf = document.getElementById("box1");

// evento de clic al div
disgraf.addEventListener("click", function () {
    // Redirecciona a una URL cuando se hace clic
    window.location.href = "https://www.google.com/";
});

var disind = document.getElementById("box2");

//evento de clic al div
disind.addEventListener("click", function () {
    // Redirecciona a una URL cuando se hace clic
    window.location.href = "/../index.html";
});


//*PARA SELECCIONAR LOS DIV DE PROYECTOS*//
var pop1 = document.getElementById("popu01");

// evento de clic al div
pop1.addEventListener("click", function () {
    // Redirecciona a una URL cuando se hace clic
    window.location.href = "https://www.google.com/";
});




/*evento carrusel 2!*/
var documentElemento = document;

documentElemento.addEventListener('DOMContentLoaded', function () {

    const sliderMe2 = () => {
        let currentPosition2 = 0,
            sliderItem2 = documentElemento.querySelectorAll('.slider-item2'),
            sliderItemWidth2 = window.getComputedStyle(sliderItem2[0]).flexBasis.match(/\d+\.?\d+/g),
            sliderInner2 = documentElemento.querySelector('.slider-inner2'),

            control2 = {
                next2: documentElemento.querySelector('#next2'),
                slideNext2() {
                    currentPosition2 += parseFloat(sliderItemWidth2);
                    if (currentPosition2 > limitPosition2) {
                        currentPosition2 = 0;
                    }
                    sliderInner2.style.right = currentPosition2 + '%';
                },
                prev2: documentElemento.querySelector('#prev2'),

                slidePrev2() {
                    currentPosition2 -= parseFloat(sliderItemWidth2);
                    if (currentPosition2 < 0) {
                        currentPosition2 = limitPosition2;
                    }
                    sliderInner2.style.right = currentPosition2 + '%';
                }
            },
            // Reducir el valor aquí para un desplazamiento mayor
            limitPosition2 = sliderItemWidth2 * (sliderItem2.length - Math.floor(100 / sliderItemWidth2));

        control2.next2.addEventListener('click', control2.slideNext2);
        control2.prev2.addEventListener('click', control2.slidePrev2);

        window.addEventListener("resize2", function () {
            currentPosition2 = 0;
            documentElemento.querySelector('.slider-inner2').style.right = currentPosition2 + '%';
        });
    }
    sliderMe2();

    window.addEventListener("resize2", sliderMe2);

});


/* SEGUNDO CARRUSEL DE EMPLEO*/
var documentoElemento = document;

documentoElemento.addEventListener('DOMContentLoaded', function () {

    const sliderMe3 = () => {
        let currentPosition3 = 0,
            sliderItem3 = documentoElemento.querySelectorAll('.slider-item3'),
            sliderItemWidth3 = window.getComputedStyle(sliderItem3[0]).flexBasis.match(/\d+\.?\d+/g),
            sliderInner3 = documentoElemento.querySelector('.slider-inner3'),

            control3 = {
                next3: documentoElemento.querySelector('#next3'),
                slideNext3() {
                    currentPosition3 += parseFloat(sliderItemWidth3);
                    if (currentPosition3 > limitPosition3) {
                        currentPosition3 = 0;
                    }
                    sliderInner3.style.right = currentPosition3 + '%';
                },
                prev3: documentoElemento.querySelector('#prev3'),

                slidePrev3() {
                    currentPosition3 -= parseFloat(sliderItemWidth3);
                    if (currentPosition3 < 0) {
                        currentPosition3 = limitPosition3;
                    }
                    sliderInner3.style.right = currentPosition3 + '%';
                }
            },
            // Reducir el valor aquí para un desplazamiento mayor
            limitPosition3 = sliderItemWidth3 * (sliderItem3.length - Math.floor(100 / sliderItemWidth3));

        control3.next3.addEventListener('click', control3.slideNext3);
        control3.prev3.addEventListener('click', control3.slidePrev3);

        window.addEventListener("resize3", function () {
            currentPosition3 = 0;
            documentoElemento.querySelector('.slider-inner3').style.right = currentPosition3 + '%';
        });
    }
    sliderMe3();

    window.addEventListener("resize3", sliderMe3);

});





/*  MOSTRAR DETALLE DE EMPLEO EN CADA AVISO*/
// Obtén todos los elementos con la clase "box-trigger"
const boxTriggers = document.querySelectorAll('.box-trigger');

// Recorre cada elemento y agrega eventos de mouseover y mouseout de manera dinámica
boxTriggers.forEach((boxTrigger) => {
    const squareId = boxTrigger.id.replace('box', 'square'); // Obtiene el ID del square correspondiente
    const square = document.getElementById(squareId);

    // Agrega evento de mouseover
    boxTrigger.addEventListener('mouseover', function () {
        square.style.visibility = 'visible';
        /* square.style.display = 'block'; // Muestra el square al pasar el mouse*/
    });

    // Agrega evento de mouseout
    boxTrigger.addEventListener('mouseout', function () {
        square.style.visibility = 'hidden'; // Cambia la visibilidad a oculto después de un retraso    
    });
});
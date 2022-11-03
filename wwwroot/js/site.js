let productos = [];
let carrito;

$(document).ready(function () {
    carrito = $('#carrito');

    carrito.html('');
});

function AñadirObjeto(id) {
    if (productos.every(p => p.id != id)) {
        let producto = { id: id, cantidad: 1 };
        productos.push(producto);
    } else {
        productos.find(p => p.id === id).cantidad += 1;
    }
    console.log(productos.find(p => p.id === id))
    ImprimirProductos();
}

function QuitarObjeto(id) {
    let producto = productos.find(p => p.id === id);

    if (producto) {
        if (producto.cantidad > 1) {
            productos.find(p => p.id === id).cantidad -= 1;
        } else {
            productos = productos.filter(p => p.id != id);
        }

    }
    ImprimirProductos();
}


function ImprimirProductos() {
    carrito.html('');
    productos.forEach(function (e) {
        let htmlstring = '<div class="card" ><div class="card-header" > Nombre < /div>< div class="card-body" > <img style="width: 100%;" src = "/imgs/coca.webp" /> </div>< div class="card-footer text-dark" ><p>' + e.cantidad
            + '</p><button id="4" class="btn btn-primary" onclick = "AñadirObjeto(' + e.id + ')" > + < /button>'
            + '<button id="4" class="btn btn-danger" onclick = "QuitarObjeto(' + e.id + ')" > - < /button>< /div>< /div>';
        carrito.html(carrito.html() + htmlstring);

    });



}


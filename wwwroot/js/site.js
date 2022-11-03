let productos = [];
let carrito;

$(document).ready(function () {
    carrito = $('#carrito');

    carrito.html('');
});

function AñadirObjeto(id) {
    if (productos.every(p => p.ProductId != id)) {
        let producto = { ProductId: id, Amount: 1 };
        productos.push(producto);
    } else {
        productos.find(p => p.ProductId === id).Amount += 1;
    }
    ImprimirProductos();
}

function QuitarObjeto(id) {
    let producto = productos.find(p => p.ProductId === id);

    if (producto) {
        if (producto.Amount > 1) {
            productos.find(p => p.ProductId === id).Amount -= 1;
        } else {
            productos = productos.filter(p => p.ProductId != id);
        }

    }
    ImprimirProductos();
}


function ImprimirProductos() {
    carrito.html('');
    productos.forEach(function (e) {
        let htmlstring = '<div class="card" ><div class="card-header">Nombre</div><div class="card-body"><img style="width: 100%;" src = "/imgs/coca.webp"/> </div><div class="card-footer text-dark" >'
            + '<button id="4" class="btn btn-primary" onclick = "AñadirObjeto(' + e.ProductId + ')" > + </button><input value="' + e.Amount
            + '"/><button id="4" class="btn btn-danger" onclick = "QuitarObjeto(' + e.ProductId + ')" > - </button></div></div>';
        carrito.html(carrito.html() + htmlstring);

    });

}

async function BuyProducts() {
    try {
        const response = await fetch('/Orders/BuyProducts', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(productos),
        });
        if (response.ok) {
            //return json
            console.log(response);
        } else {
            //
        }
    } catch (e) {
        console.log(e);
    }
}

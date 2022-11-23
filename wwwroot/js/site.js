let productos = [];
let carrito;

$(document).ready(function () {
    carrito = $('#carrito');

    carrito.html('');
});

function AñadirObjeto(id, name, price) {
    if (productos.every(p => p.ProductId != id)) {
        let producto = { ProductId: id, Amount: 1, Name: name, Price: price };
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
        let htmlstring = ` <div class="item">
            <div class="info">
                <div>
                    <p class="name">`+e.Name+`</p>
                    <div>
                        <p class="price">`+e.Price+`</p>
                        <div class="btns">
                            <input class="less" type="button"  onclick = "QuitarObjeto(` + e.ProductId + `)" value="-"/>
                            <input class="number" type="number" name="" id="" value="`+ e.Amount+`"/>
                            <input class="plus" type="button" onclick = "AñadirObjeto(` + e.ProductId + `)" value="+"/>
                        </div>
                    </div>
                </div>
                <div class="total">
                    <p>`+ e.Price +`</p>
                </div>  

            </div>
        </div>` 
         

        let htmlstring2 = '<div class="card" style="max-height:20rem" ><div class="card-header">' + e.Name + '</div><div class="card-body"><img style="max-height: 10vh;" src = "/imgs/coca.webp"/>'
            + '</div><div class="card-footer form-group text-dark" ><div class="form-group" style="display: flex;">'
            + '<button id="4" class="btn btn-primary" onclick = "AñadirObjeto(' + e.ProductId + ')" > + </button><input class="form-control" value="' + e.Amount
            + '"/><button id="4" class="btn btn-danger" onclick = "QuitarObjeto(' + e.ProductId + ')" > - </button></div></div></div>';
        carrito.html(carrito.html() + htmlstring);

    });

    $('#Total').html(productos.reduce((partialSum, a) => partialSum + a.Price * a.Amount, 0));
}

async function BuyProducts() {
    if (productos.length != 0) {
        try {

            const response = await fetch('/Orders/BuyProducts', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(productos),
            });
            if (response.ok) {
                productos = [];
                ImprimirProductos();
            } else {
                //
            }
        } catch (e) {
            console.log(e);
        }
    }
}

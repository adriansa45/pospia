using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public enum Category
    {

        Botana = 0,
        Refresco = 1,
        [Display(Name = "Bebidas Alcoholica")]
        BebidasAlcoholica = 2,
        Galletas = 3,
        [Display(Name = "Pan Dulce")]
        PanDulce = 4,
        Otros =5
    }
}

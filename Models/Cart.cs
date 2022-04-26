using AppDev1.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppDev1.Models
{
    public class Cart
    {
        public string UserId { get; set; }

        public string BookIsbn { get; set; }

        public AppUser? User { get; set; }


    }
}
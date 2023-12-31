using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineFood.Models

{
    public class RestaurantEditViewModel
    {

        public long RestaurentId { get; set; }

        [Required(ErrorMessage = "Restaurant Name is required.")]
        [Display(Name = "Restaurant Name")]
        public string RestaurentName { get; set; }

        public string RestaurentLogoUrl {  get; set; }  

        public bool RestaurentAvailable { get; set; }

        public long RestaurentAdminId { get; set; }

    }
}
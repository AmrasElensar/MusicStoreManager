using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MusicStoreManager.Entities
{
    public partial class Order
    {
        public Order()
        {
            //OrderDetail = new HashSet<OrderDetail>();
            OrderDetail = new List<OrderDetail>();
        }

        [BindNever]
        public int OrderId { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Please enter your username")]
        [Display(Name = "Username")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name="First Name")]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your address")]
        [Display(Name = "Address")]
        [StringLength(100)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Please enter your city")]
        [StringLength(50)] 
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please select your state")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Please enter your postal code")]
        [StringLength(50)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Please select your country")]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [StringLength(25)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter your phone number")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[
                            \x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])
                            *"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:
                            (?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|
                            [a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b
                            \x0c\x0e-\x7f])+)\])", ErrorMessage = "The email address entered is not valid")]
        public string Email { get; set; }

        [BindNever]
        [ScaffoldColumn(false)]
        public decimal Total { get; set; }

        public List<OrderDetail> OrderDetail { get; set; }
    }
}

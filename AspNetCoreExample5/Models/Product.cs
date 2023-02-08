using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AspNetCoreExample5.Models
{
    public class Product
    {
        public int Id { get; set; }

        [DisplayName("名稱")]
        [Required(ErrorMessage = "名稱不能為空")]
        public String Name { get; set; }

        [DisplayName("圖片")]
        [ValidateNever]
        public String? ImageUrl { get; set; }
    }
}
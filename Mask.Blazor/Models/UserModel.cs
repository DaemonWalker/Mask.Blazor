using Mask.Blazor.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Mask.Blazor.Models
{
    public class UserModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Tel { get; set; }
        [Required]
        public string IDCard { get; set; }
        [Required]
        public string ShopID { get; set; }

        [ConvertToIgnore]
        public string LastAppointmentDate { get; set; } = "2000/1/1";
        [ConvertToIgnore]
        public string AppointmentCode { get; set; }

    }
}

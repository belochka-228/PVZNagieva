using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
//using WPF_Nagieva.DTO;

namespace WPF_Nagieva.DTO
{
    public partial class ProductResponse
    {
        public string ImagePathcover  => ImagePath ?? "/Images/img1.jpg";
    }
}

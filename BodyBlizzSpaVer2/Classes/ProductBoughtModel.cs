using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyBlizzSpaVer2.Classes
{
    public class ProductBoughtModel
    {
        public string ID { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductPrice { get; set; }

        public string ClientID { get; set; }

        public string ClientName { get; set; }

        public string DateBought { get; set; }

        public string isDiscounted { get; set; }

        public string DiscountID { get; set; }

        public string isDeleted { get; set; }

        public string Total { get; set; }

    }
}

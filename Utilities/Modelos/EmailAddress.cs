using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smadot.Utilities.Modelos
{
    public class EmailAddress
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public EmailAddress()
        {

        }

        public EmailAddress(string name, string address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}

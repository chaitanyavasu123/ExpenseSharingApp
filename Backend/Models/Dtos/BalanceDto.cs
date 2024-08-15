using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dtos
{
    public class BalanceDto
    {
        public decimal AmountOwed { get; set; }
        public decimal AmountOwedTo { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using apiTest.App_Code;

namespace apiTest.App_Code
{
    public class Coupon
    {
        public string Name { get; set; }
        public List<BetMatch> Matchs { get; set; }
    }
}
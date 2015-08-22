using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace apiTest.App_Code
{
    public class DailyMatch
    {
        public string Time { get; set; }
        public string HomeTeam { get; set; }
        public string DisplacementTeam { get; set; }
        public string Score { get; set; }
        public string Minute { get; set; }
        public string Type { get; set; }
    }
}
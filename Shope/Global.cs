﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shope
{
    static public class Global 
    {
        // chek if the user is admin
        public static int Admin = 0;

        public static int sessionID = 0;
    }
}

﻿using Com.ChinaPalmPay.Platform.RentCar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaPalmPay.Platform.RentCar.IBLLS
{
  public  interface IPayHandler
    {
      Recharge pay(Recharge recharge);
    }
}

﻿using Com.ChinaPalmPay.Platform.RentCar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaPalmPay.Platform.RentCar.IMessaging
{
  public  interface IExistUserReg
    {
      void Send(UserLogin log);
      UserLogin Receive();
    }
}

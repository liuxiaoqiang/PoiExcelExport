﻿using Com.ChinaPalmPay.Platform.RentCar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaPalmPay.Platform.RentCar.IBLLS
{
    public interface IOrderHandler
    {
        //create order
        Order createOrder(Order order,out string msg);
        //select order
        IList<Order> selectOrder(string userId);
        int selectOrderNum(string userId);
        bool cancelOrder(string orderId);
        //查询有效订单车辆所在租赁点经纬度
        Station queryLocation(string carId,string userId);
        //订单授权给新用户
        bool OrderAuthorization(string UserId,string OrderId,string Telphone,out string msg);
        bool cancelAuthorization(string UserId, string OrderId);
    }
}

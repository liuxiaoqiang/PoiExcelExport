﻿using Com.ChinaPalmPay.Platform.RentCar.BLLFacs;
using Com.ChinaPalmPay.Platform.RentCar.Common;
using Com.ChinaPalmPay.Platform.RentCar.IBLLS;
using Com.ChinaPalmPay.Platform.RentCar.Model;
using Com.ChinaPalmPay.Platform.RentCar.Model.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace com.chinapalmpay.platform.RentCars.Controllers
{
    public class OrderController : BaseController
    {
        private static readonly IOrderHandler orderbll = BllAccess.CreateOrderService();
        //订单授权
        public ActionResult Authorize(string UserId, string OrderId, string Telphone)
        {
                DefaultResult res = new DefaultResult();
            try
            {
                string msg = null;
                bool result = orderbll.OrderAuthorization(UserId, OrderId, Telphone, out  msg);
                if(result){
                    res.Code = "0000";
                    res.Data = result;
                    res.Message = msg;
                    LogHelper.OutPut(this.Url.RequestContext,result);
                }
                else
                {
                    res.Code = "0001";
                    res.Data = "";
                    res.Message = msg;
                    LogHelper.OutPut(this.Url.RequestContext, result);
                }
            }catch(Exception e){
                res.Code = "0201";
                res.Data = "";
                res.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext,e);
            }
            return Json(res);
        }
       //取消授权
        public ActionResult cancelAuthorize(string UserId,string OrderId)
        {
            DefaultResult res = new DefaultResult();
            try
            {
                bool result = orderbll.cancelAuthorization(UserId, OrderId);
                if (result)
                {
                    res.Code = "0000";
                    res.Data = result;
                    res.Message = "取消授权成功";
                    LogHelper.OutPut(this.Url.RequestContext, result);
                }
                else
                {
                    res.Code = "0001";
                    res.Data = "";
                    res.Message = "取消授权失败";
                    LogHelper.OutPut(this.Url.RequestContext, "取消授权失败");

                }
            }
            catch (Exception e)
            {
                res.Code = "0201";
                res.Data = "";
                res.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext,e);
            }
            return Json(res);
        }
        /*create order*/
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Create(Order order)
        {
            DefaultResult res = new DefaultResult();
            string msg="";
            try
            {
                if (!String.IsNullOrWhiteSpace(order.Time) && !String.IsNullOrWhiteSpace(order.UserID) &&!String.IsNullOrWhiteSpace(order.CarID))
                {
                    Order orders =orderbll.createOrder(order,out msg);
                    if (orders != null)
                    {
                        res.Code = "0000";
                        res.Data = orders;
                        res.Message = msg;
                        LogHelper.OutPut(this.Url.RequestContext, orders);
                    }
                    else
                    {
                        res.Code = "0001";
                        res.Data = "";
                        LogHelper.OutPut(this.Url.RequestContext, "消息创建失败");
                        res.Message = msg;
                    }
                }
                else {
                    res.Code = "0101";
                    res.Data = "";
                    res.Message = msg;
                }
            }
            catch(Exception e)
            {
                res.Code = "0201";
                res.Data = "";
                res.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext,e);
            }
                return Json(res);
        }
        /*订单查询*/
        public ActionResult Select()
        {
            return View();
        }
        //根据用户id查询订单
        [HttpPost]
        public ActionResult Select(Order order)
        {
            DefaultResult res = new DefaultResult();
            try
            {
                if (!String.IsNullOrWhiteSpace(order.UserID))
                {
                    IList<Order> orders = orderbll.selectOrder(order.UserID);
                    if (orders != null && orders.Count()>0)
                    {
                        res.Code = "0000";
                        res.Data = orders;

                        res.Message = "查询订单成功";
                        LogHelper.OutPut(this.Url.RequestContext, orders);



                    }
                    else
                    {
                        res.Code = "0001";
                        res.Data = "";
                        res.Message = "没有订单历史记录";
                        LogHelper.OutPut(this.Url.RequestContext, "没有订单历史记录");
                    }
                }
                else {
                    res.Code = "0101";
                    res.Data = "";
                    res.Message = "收到参数为空";
                }
                return Json(res);
            }
            catch(Exception e)
            {
                res.Code = "0201";
                res.Data = "";
                res.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext,e);
                return Json(res);
            }
        }
        //根据用户id查询订单数
        public ActionResult SelectNum()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SelectNum(Order order)
        {
            DefaultResult res = new DefaultResult();
            try
            {
                if (!String.IsNullOrWhiteSpace(order.UserID))
                {
                    int orderNum = orderbll.selectOrderNum(order.UserID);
                        res.Code = "0000";
                        res.Data = orderNum;
                        LogHelper.OutPut(this.Url.RequestContext, orderNum);
                        res.Message = "查询订单数成功";
                }
                else
                {
                    res.Code = "0101";
                    res.Data = "";
                    res.Message = "收到参数为空";
                    LogHelper.OutPut(this.Url.RequestContext, "收到参数为空");
                }
                return Json(res);
            }
            catch(Exception e)
            {
                res.Code = "0201";
                res.Data = "";
                res.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext,e);
                return Json(res);
            }
        }
       /*订单撤销*/
        //param 订单id
        public ActionResult CancelOrder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CancelOrder(string OrderID)
        {
            DefaultResult def = new DefaultResult();
            try {
                if (!String.IsNullOrWhiteSpace(OrderID))
                {
                    if (orderbll.cancelOrder(OrderID))
                    {
                        def.Code = "0000";
                        def.Data = OrderID;
                        def.Message = "成功取消订单";
                        LogHelper.OutPut(this.Url.RequestContext, OrderID);
                    }
                    else
                    {
                        def.Code = "0001";
                        def.Data = OrderID;
                        def.Message = "取消订单失败";
                        LogHelper.OutPut(this.Url.RequestContext, "取消订单失败");
                    }
                }
                else
                {
                    def.Code = "0101";
                    def.Data = OrderID;
                    def.Message = "输入参数格式错误,取消订单失败";
                }
            }catch(Exception e){
                def.Code = "0201";
                def.Data = OrderID;
                def.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext,e);
            }
                return Json(def);
        }
        //根据用户id和订单号查询当前唯一有效订单
        public ActionResult Query()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Query(string carId, string userId)
        {
            DefaultResult def = new DefaultResult();
            try
            {
                if (!String.IsNullOrWhiteSpace(carId) && !String.IsNullOrWhiteSpace(userId))
                {
                    Station sta = orderbll.queryLocation(userId, carId);
                    if (sta != null)
                    {
                        def.Code = "0000";
                        def.Data = new {Latitude=sta.Latitude,Longitude=sta.Longitude};
                        def.Message = "查询有效订单车辆位置成功";
                        LogHelper.OutPut(this.Url.RequestContext, new { Latitude = sta.Latitude, Longitude = sta.Longitude });
                    }
                    else
                    {
                        def.Code = "0001";
                        def.Data = "";
                        def.Message = "查询有效订单车辆位置失败";
                        LogHelper.OutPut(this.Url.RequestContext, "查询有效订单车辆位置失败");

                    }
                }
                else
                {
                    def.Code = "0101";
                    def.Data = "";
                    def.Message = "查询有效订单车辆位置参数有空值";
                }
               
            }catch(Exception ex){
                def.Code = "0201";
                def.Data = "";
                def.Message = "系统繁忙,请联系客服";
                LogHelper.Exception(this.Url.RequestContext, ex);
            }
            return Json(def);
        }
    }
}

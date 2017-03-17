﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Com.ChinaPalmPay.Platform.RentCar.IBLLS;
using Com.ChinaPalmPay.Platform.RentCar.BLLFacs;
using Com.ChinaPalmPay.Platform.RentCar.Model;
using Com.ChinaPalmPay.Platform.RentCar.Common;
using Com.ChinaPalmPay.Platform.RentCar.Model.OperationResult;

namespace com.chinapalmpay.platform.RentCars.Controllers
{
    public class ZSCController : BaseController
    {
        //
        // GET: /ZSC/
        private static readonly IZSCHandler zscbll = BllAccess.CreateZSCService();
        public ActionResult SelectStation()
        {
            return View();

        }
        //根据地区返回租赁点
        [HttpPost]
        public ActionResult SelectStation(District district)
        {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            DefaultResult ret = new DefaultResult();
            try
            {
                //根据地区编号查询租赁点
                    IList<Station> stations = zscbll.findStationHandler(district.DistrictID);
                    if (stations != null && stations.Count() > 0)
                    {
                        ret.Code = "0000";
                        ret.Message = "station查询成功";
                        ret.Data = stations;
                        LogHelper.OutPut(this.Url.RequestContext, stations);
                    }
                    else
                    {
                        ret.Code = "0001";
                        ret.Message = "station查询失败";
                        ret.Data = "";
                        LogHelper.OutPut(this.Url.RequestContext, "station查询失败");
                    }
            }
            catch (Exception ex)
            {
                ret.Code = "0201";
                ret.Message = "系统繁忙,请联系客服";
                ret.Data = "";
                LogHelper.Exception(this.Url.RequestContext, ex);
            }
           
            return Json(ret);
        }
        //根据租赁点查询车辆信息
        public ActionResult SelectCar()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SelectCar(string id)
        {
            LogHelper.Input(id);
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            DefaultResult ret = new DefaultResult();
            try
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    IList<Car> cars = zscbll.findCarHandler(int.Parse(id));
                    if (cars != null && cars.Count() > 0)
                    {
                        ret.Code = "0000";
                        ret.Message = "car查询成功";
                        ret.Data = cars;
                        LogHelper.OutPut(this.Url.RequestContext, cars);
                    }
                    else
                    {
                        ret.Code = "0001";
                        ret.Message = "该租赁点没有可用车";
                        ret.Data = cars;
                        LogHelper.OutPut(this.Url.RequestContext, "该租赁点没有可用车");
                    }
                }
                else
                {
                    ret.Code = "0200";
                    ret.Message = "接受id参数为空";
                    ret.Data = "0000";
                }
            }
            catch (Exception ex)
            {
                ret.Code = "0100";
                ret.Message ="系统繁忙,请联系客服";
                ret.Data = "0000";
                LogHelper.Exception(this.Url.RequestContext, ex);
            }
            return Json(ret);
        }

        public ActionResult SelectDistrict()
        {
            return View();
        }
        //根据城市名获取地区列表
        [HttpPost]
        public ActionResult SelectDistrict(City c)
        {
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            DefaultResult ret = new DefaultResult();
            try
            {
                if (!String.IsNullOrWhiteSpace(c.CityName))
                {
                    IList<District> dis = zscbll.findZoneHandler(c.CityName);
                    if (dis != null && dis.Count() > 0)
                    {
                        ret.Code = "0000";
                        ret.Message = "查询成功";
                        ret.Data = dis;
                        LogHelper.OutPut(this.Url.RequestContext, dis);
                    }
                    else
                    {
                        ret.Code = "0001";
                        ret.Message = "查询失败";
                        ret.Data = "";
                        LogHelper.OutPut(this.Url.RequestContext, "查询失败");
                    }

                }
                else
                {
                    ret.Code = "0101";
                    ret.Message = "接受id参数为空";
                    ret.Data = "0000";
                }
            }
            catch (Exception ex)
            {
                ret.Code = "0201";
                ret.Message = "系统繁忙,请联系客服";
                ret.Data = "";
                LogHelper.Exception(this.Url.RequestContext, ex);
            }
            return Json(ret);
        }
        public ActionResult SelectCarById()
        {
            return View();
        }
        //根据车id查询车信息
        [HttpPost]
        public ActionResult SelectCarById(string id)
        {
            DefaultResult ret = new DefaultResult();
            Response.AddHeader("Access-Control-Allow-Origin", "*");
            try
            {
                if (!String.IsNullOrWhiteSpace(id))
                {
                    Car car= zscbll.findCarByIdHandler(id);
                    if (car!= null)
                    {
                        ret.Code = "0000";
                        ret.Message = "查询车辆成功";
                        ret.Data = car;
                        LogHelper.OutPut(this.Url.RequestContext, car);
                    }
                    else
                    {
                        ret.Code = "0001";
                        ret.Message = "查询车辆失败";
                        ret.Data = "";
                        LogHelper.OutPut(this.Url.RequestContext, "查询失败");
                    }

                }
                else
                {
                    ret.Code = "0101";
                    ret.Message = "接受id参数为空";
                    ret.Data = "0000";
                }
            }
            catch (Exception ex)
            {
                ret.Code = "0201";
                ret.Message = "系统繁忙,请联系客服";
                ret.Data = "";
                LogHelper.Exception(this.Url.RequestContext, ex);
            }
            return Json(ret);
        }
    }
}

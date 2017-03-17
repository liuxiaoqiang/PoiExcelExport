﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Com.ChinaPalmPay.Platform.RentCar.IDAL;
using System.Configuration;
using Com.ChinaPalmPay.Platform.RentCar.DALFactory;
using Com.ChinaPalmPay.Platform.RentCar.Model;
using System.Web;
using Com.ChinaPalmPay.Platform.RentCar.CacheDependencyFactory;
using System.Web.Caching;
namespace Com.ChinaPalmPay.Platform.RentCar.DataProx
{
    public class ZSCDataProxy : IZSCManager
    {//查询结果加入缓存
        private static readonly int UserRegTimeout = int.Parse(ConfigurationManager.AppSettings["UserRegCacheDuration"]);
        private static readonly bool enableCaching = bool.Parse(ConfigurationManager.AppSettings["EnableCaching"]);
        private static readonly IZSCManager ZSCManager = DataAccess.CreateZSCCacheManager();
        // Get an instance of the Category DAL using the DALFactory
        // Making this static will cache the DAL instance after the initial load
        private static readonly IZSCManager manager = DataAccess.CreateZSCDbManager();
        public IList<Station> findStation(long zoneId)
        {
            IList<Station> stations = null;
            string group_key = "Select_Station" + zoneId;
            //如果不允许缓存
            if (enableCaching)
            {
                stations = HttpRuntime.Cache[group_key] as IList<Station>;
            }
            if (stations == null)
            {
                stations = manager.findStation(zoneId);
            }
            // Check if the data exists in the data cache
            if (stations != null && stations.Count() > 0)
            {
                //// If the data is not in the cache then fetch the data from the business logic tier
                AggregateCacheDependency cd = DependencyFacade.GetStationSelDependency();
                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                HttpRuntime.Cache.Add(group_key, stations, cd, DateTime.Now.AddHours(UserRegTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            return stations;

        }

        public IList<Car> findCar(int id)
        {
            IList<Car> cars = null;
            //string group_key = "Select_Car " + id;

            //if (enableCaching)
            //{
            //    cars = HttpRuntime.Cache[group_key] as IList<Car>;

            //}
            //if (cars == null)
            //{
                cars = manager.findCar(id);
           // }

            // Check if the data exists in the data cache
            //if (cars != null && cars.Count() > 0)
            //{
            //    //// If the data is not in the cache then fetch the data from the business logic tier
            //    AggregateCacheDependency cd = DependencyFacade.GetCarSelDependency();
            //    // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
            //    HttpRuntime.Cache.Add(group_key, cars, cd, DateTime.Now.AddHours(UserRegTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            //}
            return cars;
        }
        public IList<District> findDistrict(string city)
        {
            IList<District> dis = null;
            string group_key = "Select_District " + city;
            if (enableCaching) { 
            dis = (IList<District>)HttpRuntime.Cache[group_key];
            }
            if(dis==null){
                 dis = manager.findDistrict(city);
            }
            // Check if the data exists in the data cache
            if (dis != null&&dis.Count()>0)
            {
                //// If the data is not in the cache then fetch the data from the business logic tier
                AggregateCacheDependency cd = DependencyFacade.GetDistrictSelDependency();
                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                HttpRuntime.Cache.Add(group_key, dis, cd, DateTime.Now.AddHours(UserRegTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);
            }
            return dis;
        }




        public Car findCarByCarId(string id)
        {
            Car cars = null;
            string key= "Select_CarById" + id;
            if (enableCaching)
            {
                cars = HttpRuntime.Cache[key] as Car;
            }
            if(cars==null){
                cars = manager.findCarByCarId(id);
            }
            if(cars!=null){
                //// If the data is not in the cache then fetch the data from the business logic tier
                AggregateCacheDependency cd = DependencyFacade.GetSelCarByIdDependency();
                // Store the output in the data cache, and Add the necessary AggregateCacheDependency object
                HttpRuntime.Cache.Add(key, cars, cd, DateTime.Now.AddHours(UserRegTimeout), Cache.NoSlidingExpiration, CacheItemPriority.High, null);

            }
            return cars;
        }


        public CarStat findCarStat(string userId, string carId)
        {
            throw new NotImplementedException();
        }


        public Station findStaByPileId(string pile)
        {
            throw new NotImplementedException();
        }


        public CarStat updateCarStat(string carId)
        {
            throw new NotImplementedException();
        }


        public CarStat updateCarStat(Order order)
        {
            throw new NotImplementedException();
        }


        public CarStat addCarStat(Order order)
        {
            throw new NotImplementedException();
        }


        public CarStat addCarStat(OrderLog order)
        {
            throw new NotImplementedException();
        }
    }
}

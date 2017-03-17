﻿using Com.ChinaPalmPay.Platform.RentCar.DALFactory;
using Com.ChinaPalmPay.Platform.RentCar.IDAL;
using Com.ChinaPalmPay.Platform.RentCar.IMessaging;
using Com.ChinaPalmPay.Platform.RentCar.MessagingFactory;
using Com.ChinaPalmPay.Platform.RentCar.Model;
using Com.ChinaPalmPay.Platform.RentCar.SQLServer;
using Com.ChinaPalmPay.Platform.RentCar.MsmqMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace DBHander
{
    public class HANDLERS
    {
        private static readonly IUserReg regQueue = QueueAccess.CreateUserRegister();
        private static readonly UserOperations db = DataAccess.CreateUserDbManager();
        private static readonly IOrderManager orderDB = DataAccess.CreateOrderDbManager();
        private static readonly IOrderMsmq orderQueue = QueueAccess.CreateOrder();
        private static readonly IPayManager pays = DataAccess.CreatePayDbManager();
        private static readonly IPayMsmq msmq = QueueAccess.Pay();
        private static readonly IUserCompl msmqCom = QueueAccess.CompleteUser();
        private static readonly IUserUp msmqUserUp = QueueAccess.UpdateUser();
        private static readonly IUserChangeTel msmqUserChangeTel = QueueAccess.ChangeUserTel();
        private static readonly IUserChangePhoto msmqUserChangePhoto = QueueAccess.ChangeUserPhoto();
        private static readonly IalipayMsmq alipayQueue = QueueAccess.Alipay();
        private static readonly ICupMsmq cupQueue = QueueAccess.Cup();
        private static readonly IExistUserReg exist = QueueAccess.ExistUserReg();
        private static readonly IReturnCarMsmq returnPay = QueueAccess.ReturnCarPay();
        private static readonly IZSCManager zscManager = DataAccess.CreateZSCDbManager();
        private static readonly IWechatUserReg WechatUserRegQueue = QueueAccess.WechatUserReg();
        public HANDLERS() { }
        //启动DB线程
        //本接口用户从消息队列取出消息，进行DB存储
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                new HANDLERS().Work();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("DBHandler 开始运行！");
            System.Console.ReadKey();
        }
        public void Work()
        {
            //创建最小线程数和最小空闲线程数
            ThreadPool.SetMaxThreads(50, 100);
            ThreadPool.QueueUserWorkItem(new WaitCallback(createUser));
            ThreadPool.QueueUserWorkItem(new WaitCallback(createOrder));
            ThreadPool.QueueUserWorkItem(new WaitCallback(UserCompl));
            ThreadPool.QueueUserWorkItem(new WaitCallback(UserUpdate));
            ThreadPool.QueueUserWorkItem(new WaitCallback(UserChangeTel));
            ThreadPool.QueueUserWorkItem(new WaitCallback(UserChangePhoto));
            ThreadPool.QueueUserWorkItem(new WaitCallback(alipay));
            ThreadPool.QueueUserWorkItem(new WaitCallback(cup));
            ThreadPool.QueueUserWorkItem(new WaitCallback(existUserReg));
            ThreadPool.QueueUserWorkItem(new WaitCallback(ReturnCarPay));
            ThreadPool.QueueUserWorkItem(new WaitCallback(WechatUserReg));
            // ThreadPool.QueueUserWorkItem(new WaitCallback(createPri));
        }
        //线程实例
        //用户管理类DB
        public void createUser(object obj)
        {
            try
            {
                object[] objs = null;
                UserGroup user = null;
                UserLogin login = null;
                User info = null;
                UserRegister reg = null;
                while (true)
                {
                    user = regQueue.ReceiveUserGroup();
                    // objs = regQueue.GetAllUserMessage();
                    if (user != null)
                    {
                        Console.WriteLine("消息UserGroup" + user.ID);
                        login = regQueue.ReceiveUserLogin();
                        if (login != null)
                        {
                            Console.WriteLine("消息UserLogin" + login.LoginID);
                            info = regQueue.ReceiveUser();
                            if (info != null)
                            {
                                Console.WriteLine("消息User" + info.PhoneNumber);
                                reg = regQueue.ReceiveUserRegister();
                                if (reg != null)
                                {
                                    Console.WriteLine("消息UserRegister" + reg.UserId);
                                    db.Create(user);
                                    db.CreateOrUpdate(login);
                                    db.CreateOrUpdate(info);
                                    db.Create(reg);
                                    Console.WriteLine("消息DB .." + reg.UserId);
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("消息DB异常" + ex.ToString());
            }
        }
        public void WechatUserReg(object obj)
        {
            try
            {
                object[] objs = null;
                UserGroup user = null;
                UserLogin login = null;
                UserLogin AppLogin = null;
                User info = null;
                UserRegister reg = null;
                while (true)
                {
                    user = WechatUserRegQueue.ReceiveUserGroup();
                    // objs = regQueue.GetAllUserMessage();
                    if (user != null)
                    {
                        Console.WriteLine("消息UserGroup" + user.ID);
                        login = WechatUserRegQueue.ReceiveUserLogin();
                        if (login != null) 
                        {
                            Console.WriteLine("消息UserLogin" + login.LoginID);
                            AppLogin=WechatUserRegQueue.ReceiveUserLogin();
                            if (AppLogin!=null)
                            {
                                Console.WriteLine("消息AppUserLogin" + AppLogin.LoginID);
                            info = WechatUserRegQueue.ReceiveUser();
                            if (info != null)
                            {
                                Console.WriteLine("消息User" + info.PhoneNumber);
                                reg = WechatUserRegQueue.ReceiveUserRegister();
                                if (reg != null)
                                {
                                    Console.WriteLine("消息UserRegister" + reg.UserId);
                                    db.Create(user);
                                    db.CreateOrUpdate(login);
                                    db.CreateOrUpdate(AppLogin);
                                    db.CreateOrUpdate(info);
                                    db.Create(reg);
                                }
                            }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("消息DB异常" + ex.ToString());
            }
        }
        //create order db
        public void createOrder(object obj)
        {
            try
            {
                Order order = null;
                UserAuthorization aut = null;
                while (true)
                {
                    order = orderQueue.Receive();
                    if (order != null)
                    {
                        aut = orderQueue.ReceiveUserAuth();
                        if (aut != null)
                        {
                            Console.WriteLine("生成订单消息:用户ID为" + order.UserID);
                            Console.WriteLine("生成订单履历");
                            Console.WriteLine("增加车状态履历");
                            orderDB.Create(order);
                            orderDB.Add(aut);
                            //增加一条CarStat
                            zscManager.updateCarStat(order);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("createOrder消息DB异常" + ex.ToString());
            }
        }

        public void UserCompl(object obj)
        {
            User c = null;
            try
            {
                while (true)
                {
                    c = msmqCom.Receive();
                    if (c != null)
                    {
                        Console.WriteLine("消息User修改:" + c.Name);
                        db.CreateOrUpdate(c);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("UserCompl消息DB异常" + e.ToString());
            }
        }

        public void UserUpdate(object obj)
        {
            UserLogin c = null;
            try
            {
                while (true)
                {
                    c = msmqUserUp.Receive();
                    if (c != null)
                    {
                        Console.WriteLine("消息User修改密码:" + c.LoginPwd);
                        db.CreateOrUpdate(c);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UserUpdate消息DB异常" + ex.ToString());
            }
        }
        public void UserChangeTel(object obj)
        {
            UserLogin c = null;
            try
            {
                while (true)
                {
                    c = msmqUserChangeTel.Receive();
                    if (c != null)
                    {
                        Console.WriteLine("消息User修改手机号码:" + c.UserId);
                        db.CreateOrUpdate(c);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("消息DB异常" + ex.ToString());
            }
        }
        public void UserChangePhoto(object obj)
        {
            User user = null;
            try
            {
                while (true)
                {
                    user = msmqUserChangePhoto.Receive();
                    if (user != null)
                    {
                        Console.WriteLine("消息User头像:" + user.UserId);
                        db.userChangePhoto(user);
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("消息DB异常" + ex.ToString());
            }
        }
        public void alipay(object obj)
        {
            try
            {
                Alipay alipay = null;
                Recharge charge = null;
                // OrderLog log = null;
                while (true)
                {
                    alipay = alipayQueue.ReceiveAlipay();
                    // objs = regQueue.GetAllUserMessage();
                    if (alipay != null)
                    {
                        Console.WriteLine("支付宝付款消息Alipay" + alipay.out_trade_no);
                        charge = alipayQueue.ReceiveRecharge();
                        if (charge != null)
                        {
                            Console.WriteLine("支付宝付款消息Recharge" + charge.OrderID);
                            // log = alipayQueue.ReceiveOrderLog();
                            //  if (log != null)
                            // {
                            //Console.WriteLine("支付宝付款消息log" + log.Remark);
                            pays.Addpay(charge);
                            pays.Addalipay(alipay);
                            // pays.Addlog(log);
                            // }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("alipay消息DB异常" + ex.ToString());
            }

        }
        public void cup(object obj)
        {
            try
            {
                Cup cup = null;
                Recharge charge = null;
                //OrderLog log = null;
                while (true)
                {
                    cup = cupQueue.ReceiveCup();
                    // objs = regQueue.GetAllUserMessage();
                    if (cup != null)
                    {
                        Console.WriteLine("银联付款消息Cup" + cup.orderId);
                        charge = cupQueue.ReceiveRecharge();
                        if (charge != null)
                        {
                            Console.WriteLine("银联付款消息Recharge" + charge.OrderID);
                            // log = cupQueue.ReceiveOrderLog();
                            // if (log != null)
                            // {
                            //  Console.WriteLine("银联付款消息log" + log.Remark);
                            pays.Addpay(charge);
                            //  pays.Addlog(log);
                            pays.AddCup(cup);
                            // }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("消息DB异常" + ex.ToString());
            }

        }
        //还车扣款
        public void ReturnCarPay(object obj)
        {
            try
            {
                OrderLog log = null;
                Recharge charge = null;
                while (true)
                {
                    charge = returnPay.ReceiveRecharge();


                    if (charge != null)
                    {
                        Console.WriteLine("还车扣款：" + charge.Amount + "元");
                        log = returnPay.ReceiveOrderLog();

                        Console.WriteLine("还车扣款11：" + log.OrderID);
                        if (log != null)
                        {
                            Console.WriteLine("还车扣款修改订单履历：" + log.State);
                            pays.Addpay(charge);
                            pays.Addlog(log);
                            zscManager.addCarStat(log);
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        public void existUserReg(object obj)
        {
            UserLogin user = null;
            try
            {
                while (true)
                {
                    user = exist.Receive();
                    if (user != null)
                    {
                        Console.WriteLine("wechat已注册,app再次注册:" + user.UserId);
                        db.AddExistUserLogin(user); ;
                    }
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("消息DB异常" + ex.ToString());
            }
        }
    }
}

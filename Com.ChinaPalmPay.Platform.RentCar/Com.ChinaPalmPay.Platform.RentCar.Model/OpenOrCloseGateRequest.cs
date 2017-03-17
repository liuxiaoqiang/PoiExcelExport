﻿using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.ChinaPalmPay.Platform.RentCar.Model
{
    [Table(Name = "C_OpenOrCloseGateRequest")]
    [Serializable]
    public class OpenOrCloseGateRequest
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public Guid Id { get; set; }
        [Column]
        public string TerminalId { get; set; }
        [Column]
        public string UserId { get; set; }
        [Column]
        public string SampleTime { get; set; }
        [Column]
        public DateTime CreateTime { get; set; }
    }
}

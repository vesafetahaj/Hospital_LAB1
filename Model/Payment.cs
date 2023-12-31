﻿using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public string PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int? Patient { get; set; }
        public int? Report { get; set; }

        public virtual Patient? PatientNavigation { get; set; }
        public virtual Report? ReportNavigation { get; set; }
    }
}

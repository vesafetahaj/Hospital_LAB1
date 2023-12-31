﻿using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Model
{
    public partial class Complaint
    {
        public int ComplaintId { get; set; }
        public DateTime? ComplaintDate { get; set; }
        public string? ComplaintDetails { get; set; }
        public int? Patient { get; set; }

        public virtual Patient? PatientNavigation { get; set; }
    }
}

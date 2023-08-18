﻿using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Models
{
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime? ReservationDate { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        public int? Patient { get; set; }
        public int? Doctor { get; set; }
        public int? Administrator { get; set; }
        public int? Service { get; set; }

        public virtual Administrator? AdministratorNavigation { get; set; }
        public virtual Doctor? DoctorNavigation { get; set; }
        public virtual Patient? PatientNavigation { get; set; }
        public virtual Specialization? ServiceNavigation { get; set; }
    }
}

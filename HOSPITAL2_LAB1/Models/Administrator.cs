﻿using System;
using System.Collections.Generic;

namespace HOSPITAL2_LAB1.Models
{
    public partial class Administrator
    {
        public Administrator()
        {
            Complaints = new HashSet<Complaint>();
            ContactForms = new HashSet<ContactForm>();
            Reservations = new HashSet<Reservation>();
            Specializations = new HashSet<Specialization>();
        }
       
        public int AdminId { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? UserId { get; set; }

        public virtual AspNetUser? User { get; set; }
        public virtual ICollection<Complaint> Complaints { get; set; }
        public virtual ICollection<ContactForm> ContactForms { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Specialization> Specializations { get; set; }
    }
}

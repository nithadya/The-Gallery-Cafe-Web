//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GalleryCafe.web.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reservation
    {
        public int ReservationId { get; set; }
        public Nullable<int> UserId { get; set; }
        public System.DateTime ReservationDate { get; set; }
        public int NumberOfGuests { get; set; }
        public Nullable<bool> ParkingRequired { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string Status { get; set; }
        public int TableNumber { get; set; }
    
        public virtual User User { get; set; }
    }
}

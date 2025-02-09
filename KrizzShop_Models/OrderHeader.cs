﻿using KrizzShop_Models.ViewModels;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace KrizzShop_Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string CreatedByUserId { get; set; }
        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser CreatedBy { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public DateTime ShippingDate { get; set; }
        [Required]
        public double FinalOrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string TransactionId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
        
    }
}

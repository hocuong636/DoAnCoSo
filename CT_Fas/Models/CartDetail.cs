using System;

namespace CT_Fas.Models
{
    public class CartDetail
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int SizeId { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public virtual Cart Cart { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductSize Size { get; set; }
    }
}
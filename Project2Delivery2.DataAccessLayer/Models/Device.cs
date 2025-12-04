using System.ComponentModel.DataAnnotations;

namespace Project2Delivery2.DataAccessLayer.Models
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        public string DeviceHash { get; set; }
        public string LastIp { get; set; }
        public string LastGeo { get; set; }
        public DateTime FirstSeen { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
}

namespace Hotel_Room_Reservation_System.Models
{
    public class Reservation
    {
        public int ReservationId {  get; set; }
        public int UserId { get; set;  }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }  
        public decimal TotalAmount { get; set;  }

        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<ReservationRoom> ReservationRooms { get; set; } = new();


    }
}

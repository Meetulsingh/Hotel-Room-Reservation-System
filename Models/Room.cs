namespace Hotel_Room_Reservation_System.Models
{
    public class Room
    {
        public int RoomId{  get; set; }
        public string RoomNumber { get; set;  }
        public string RoomType { get; set;  }

        public string Description { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }

        public decimal PricePerNight { get; set;  }

        public List<ReservationRoom> ReservationRooms { get; set; }
    }
}

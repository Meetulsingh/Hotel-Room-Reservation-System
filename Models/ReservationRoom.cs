namespace Hotel_Room_Reservation_System.Models
{
    public class ReservationRoom
    {
        public int ReservationRoomId {  get; set; }
        
        public int ReservationId { get; set; }

        public int RoomId { get; set;  }
        public Reservation? Reservation { get; set; }
        public Room? Room { get; set;  }

    }
}

namespace Hotel_Room_Reservation_System.DTOs
{
    public class RoomDto
    {
        public string RoomNumber { get; set; }

        public string RoomType { get; set; }

        public string Description { get; set; }
        public decimal PricePerNight { get; set; }

        public int Capacity { get; set; }
        public bool IsActive { get; set; }

    }
}

namespace Hotel_Room_Reservation_System.DTOs
{
    public class AddReservationDto
    {
        public int UserId { get; set; }
        public List<int> RoomIds { get; set; } = new();

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}

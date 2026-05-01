namespace Hotel_Room_Reservation_System.DTOs
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public int UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public decimal TotalAmount { get; set; }

        public List<RoomDto> Rooms { get; set; }
    }
}

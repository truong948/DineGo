namespace DineGo.Models.Enums
{
    /// <summary>
    /// Trạng thái của Phiếu Yêu Cầu (Booking/Reservation)
    /// </summary>
    public enum TrangThaiPhieu
    {
        Pending = 0,
        Confirmed = 1,
        Arrived = 2,
        Completed = 3,
        Cancelled = 4
    }
}

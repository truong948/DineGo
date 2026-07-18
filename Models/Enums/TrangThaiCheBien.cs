namespace DineGo.Models.Enums
{
    /// <summary>
    /// Trạng thái chế biến của từng món ăn trong phiếu yêu cầu
    /// </summary>
    public enum TrangThaiCheBien
    {
        ChoCungUng = 0,   // Chờ chế biến
        DangCheBien = 1,  // Đang nấu/chế biến
        DaLenMon = 2      // Đã phục vụ ra bàn
    }
}

namespace DineGo.Models.Enums
{
    /// <summary>
    /// Các trạng thái hoạt động của bàn ăn
    /// </summary>
    public enum TrangThaiBan
    {
        Trong = 0,        // Bàn trống
        DaDatTruoc = 1,   // Bàn đã được đặt trước
        DangCoKhach = 2,  // Bàn đang có khách ăn
        DangDonDep = 3    // Bàn đang dọn dẹp
    }
}

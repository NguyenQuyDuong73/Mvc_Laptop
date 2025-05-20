namespace MvcLaptop.Models
{
    public class RevenueByMonth
    {
        public int Month { get; set; }               // Tháng
        public int Year { get; set; }                // Năm (để biết thuộc năm nào)
        public decimal TotalRevenue { get; set; }    // Tổng doanh thu

        // Thuộc tính để hiển thị "Tháng 1", "Tháng 2", v.v.
        public string MonthLabel => $"Tháng {Month}";
    }
}

namespace MvcLaptop.Models
{
    public class OrderStatsByMonth
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int OrderCount { get; set; }

        public string MonthYear => $"Tháng {Month}/{Year}";
    }
}

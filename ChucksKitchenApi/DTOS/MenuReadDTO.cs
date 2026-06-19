namespace ChucksKitchenApi.DTOS
{
    public class MenuReadDTO
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }

    }
}

namespace ChucksKitchenApi.DTOS
{
    public class CategoryReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MenuReadDTO> Menus { get; set; }
    }
}

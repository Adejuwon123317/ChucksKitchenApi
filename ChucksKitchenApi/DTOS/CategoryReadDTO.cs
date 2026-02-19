namespace ChucksKitchenApi.DTOS
{
    public class CategoryReadDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MenuReadDTO> Menus { get; set; }
    }
}

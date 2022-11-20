namespace CityInfo.Models
{
    public class CityWithoutPointOfInterest
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Description { get; set; }
    }
}

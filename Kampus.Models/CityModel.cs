namespace Kampus.Models
{
    public class CityModel : Entity
    {
        public string Name { get; set; }

        public CityModel(int cityId, string name) : base(cityId)
        {
            Name = name;
        }
    }
}
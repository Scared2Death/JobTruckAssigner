namespace JobTruckAssigner.Models
{
    public class Truck
    {
        public int ID { get; set; }
        public List<char> JobTypeLetters { get; set; } = new();
        public bool HasBeenAssigned { get; set; }

    }
}
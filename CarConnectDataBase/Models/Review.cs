namespace CarConnectDataBase.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public int UserId { get; set; }
        public int CarId { get; set; }
    }
}

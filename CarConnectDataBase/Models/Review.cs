namespace CarConnectDataBase.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public int? CarId { get; set; }
        public virtual User User { get; set; }
        public virtual Car Car { get; set; }
    }
}

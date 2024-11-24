namespace TodoApp_FrontEnd.Models
{
    public class Todo
    {
        public string _id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public bool isCompleted { get; set; }
        public string userId { get; set; }
        public string createdAt { get; set; }
        public int __v { get; set; }
    }
}

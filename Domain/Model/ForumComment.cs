using BookingApp.Serializer;

namespace BookingApp.Domain.Model
{
    public class ForumComment : ISerializable
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        public ForumComment() { }

        public void FromCSV(string[] values)
        {
            Id = int.Parse(values[0]);
            ForumId = int.Parse(values[1]);
            CommentId = int.Parse(values[2]);
        }

        public string[] ToCSV()
        {
            return new string[] 
            {
                Id.ToString(),
                ForumId.ToString(), 
                CommentId.ToString() 
            };
        }
    }
}
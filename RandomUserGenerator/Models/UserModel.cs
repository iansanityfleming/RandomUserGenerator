using System;

namespace RandomUserGenerator.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string ImageUrl { get; set; }
        public ImageType ImageType { get; set; }
    }

    public enum ImageType
    {
        Thumbnail = 0,
        Medium = 1,
        Large = 2
    }
}

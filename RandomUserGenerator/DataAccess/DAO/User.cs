using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RandomUserGenerator.DataAccess.DAO
{    
    public class User : DynamoDAO
    {
        public int id { get; set; }
        public string email { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public string phone { get; set; }
        public string largeUrl { get; set; }
        public string mediumUrl { get; set; }
        public string thumbnailUrl { get; set; }
    }
}

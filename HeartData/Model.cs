using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeartData
{
    class Model
    {
    }

    public class Heart
    {
        public string title { set; get; }
        public string name { set; get; }
        public DateTime begin { set; get; }
        public DateTime end { set; get; }

        public int id { set; get; }
        public int pubid { set; get; }
        public string participator { set; get; }
        public string contact { set; get; }
        public string bgimage { set; get; }
        public string content { set; get; }
        public int station { set; get; }
    }

    public class User
    {
        public string LoginName { get; set; }
        public DateTime SignupDate { get; set; }
        public bool UseEmail { get; set; }
        public int Status { get; set; }
    }

    public class NewHeart
    {
        public string Title { set; get; }
        public string Puber { set; get; }
        public string Joiner { set; get; }
        public string Contact { set; get; }
        public string FinishDate { set; get; }
        public string HeartContent { set; get; }
    }
}

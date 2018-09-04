using System;
using Rx.Net.Plus;

namespace Rx_Tests
{
    [Serializable]
    [Equals]
    public class Info
    {
        public string Title { get; set; } = "Mr";
        public double Height { get; set; } = 76.5;
    }


    [Serializable]
    [Equals]
    public class User
    {
        public RxVar<string> Name = "John".ToRxVar();
        public RxVar<bool> IsMale = true.ToRxVar();
        public RxVar<int> Age = 16.ToRxVar();
        public RxVar<Info> InfoUser = new Info().ToRxVar();

        public User()
        {
            Name.IsDistinctMode = false;
        }
    }
}
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientModel
{
    public class DummyClient : IComparable<DummyClient>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public DateTime ModificationDateTime { get; set; }

        public DummyClient()
        {
            FirstName = "unknown";
            LastName = "unknown";
            Country = "unknown";
            Region = "Rest";
            ModificationDateTime = DateTime.Now;
        }

        public DummyClient(string firstName, string lastName, string country, string region)
        {
            if (firstName.Equals("") || firstName == null)
                throw (new ArgumentException());
            FirstName = firstName;

            if (lastName.Equals("") || firstName == null)
                throw (new ArgumentException());
            LastName = lastName;

            Country = country;
            Region = region;
            ModificationDateTime = DateTime.Now;
        }

        public override string ToString()
        {
            return FirstName + " " + LastName + " ";
        }

        public int CompareTo(DummyClient other)
        {
            if (this.ModificationDateTime.CompareTo(other.ModificationDateTime) != 0)
                return (other.ModificationDateTime.CompareTo(this.ModificationDateTime));
            if (this.LastName.CompareTo(other.LastName) != 0)
                return (this.LastName.CompareTo(other.LastName));
            if (this.FirstName.CompareTo(other.FirstName) != 0)
                return (this.FirstName.CompareTo(other.FirstName));
            if (this.Region.CompareTo(other.Region) != 0)
                return (this.Region.CompareTo(other.Region));
            if (this.Country.CompareTo(other.Country) != 0)
                return (this.Country.CompareTo(other.Country));
            return 0;
        }
    }

    public class DummyClientComparer : Comparer<DummyClient>
    {
        public override int Compare(DummyClient a, DummyClient b)
        {
            if (a.ModificationDateTime.CompareTo(b.ModificationDateTime) != 0)
                return (a.ModificationDateTime.CompareTo(b.ModificationDateTime));
            if (a.LastName.CompareTo(b.LastName) != 0)
                return (a.LastName.CompareTo(b.LastName));
            if (a.FirstName.CompareTo(b.FirstName) != 0)
                return (a.FirstName.CompareTo(b.FirstName));
            if (a.Region.CompareTo(b.Region) != 0)
                return (a.Region.CompareTo(b.Region));
            if (a.Country.CompareTo(b.Country) != 0)
                return (a.Country.CompareTo(b.Country));
            return 0;
        }
    }
}

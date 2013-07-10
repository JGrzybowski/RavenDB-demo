using System.Collections.Generic;
using ClientDAL;

namespace ShardingMvcDemo.Raven
{
    public class RavenDbConnectionManager : IRavenDbConnectionManager
    {
        public List<string> GetConnectionStrings()
        {
            return new List<string> {"EU", "USA", "Rest"};
        }

        public List<string> GetReplicationStrings()
        {
            return  new List<string> {"Orders1", "Orders1"};
        }
    }
} 
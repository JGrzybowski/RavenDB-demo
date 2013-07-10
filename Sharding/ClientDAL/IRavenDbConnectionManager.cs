using System.Collections.Generic;

namespace ClientDAL
{
    public interface IRavenDbConnectionManager
    {
        List<string> GetConnectionStrings();
        List<string> GetReplicationStrings();
    }
}

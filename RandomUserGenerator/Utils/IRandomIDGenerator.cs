using RandomUserGenerator.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomUserGenerator.Utils
{
    public interface IRandomIDGenerator
    {
        int GetRandomID();
        IEnumerable<int> GetRandomIDs(int numberRequired);
    }
}

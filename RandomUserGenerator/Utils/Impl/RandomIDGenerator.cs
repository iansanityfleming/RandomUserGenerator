using RandomUserGenerator.Models;
using RandomUserGenerator.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RandomUserGenerator.Utils
{
    public class RandomIDGenerator : IRandomIDGenerator
    {
        public int GetRandomID()
        {
            return new Random().Next(Constants.MaximumUsers);
        }

        public IEnumerable<int> GetRandomIDs(int numberRequired)
        {            
            var random = new Random();
            HashSet<int> numbers = new HashSet<int>();

            int number;
            for (int i = 0; i < Math.Min(numberRequired, Constants.MaximumUsers); i++)
            {
                do
                {
                    number = random.Next(Constants.MaximumUsers);
                } while (numbers.Contains(number));
                numbers.Add(number);
            }
            return numbers;
        }
    }
}

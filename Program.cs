using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Assignment01
{
    class PrimeSieveSolver
    {
        private int _halfLimit;
        private int _maxNumThreads;

        public PrimeSieveSolver(int limit, int maxNumThreads)
        {            
            _halfLimit = limit / 2;
            _maxNumThreads = maxNumThreads;
        }

        public LinkedList<int> FindPrimes()
        {
            LinkedList<int> primeList = new LinkedList<int>();

            // We use half of the limit of 100000000 to create the bool array because we only need to evaluate half of those numbers since even numbers will never be prime
            // The bool array is initially set entirely to false by default
            bool[] primeArray = new bool[_halfLimit];

            // We know 2 is the only even number that is prime so it should immediately be added to the list of primes
            primeList.AddLast(2);

            // This ensures that the maximum number of threads that the Parallel.For loop below will use is 8
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = _maxNumThreads
            };

            // Sieve of Eratosthenes follows below
            int sqrtLimit = (int)Math.Sqrt(_halfLimit);

            // Executing the calculations to populate the prime array in parallel rather than synchronously 
            Parallel.For
            (
                1, sqrtLimit, options, i =>
                {
                    // We use a lock here to ensure this chunk of code is protected and can only ever be executed by one thread at a time
                    lock (this)
                    {
                        if (!primeArray[i])
                        {
                            // Because we only have an array of size limit / 2, some extra math is required to convert the number value (e.g. 3) to its corresponding index (1)
                            int factor = (int)(2 * (i + 0.5));
                            int multValue = factor * factor;

                            while (multValue < (_halfLimit * 2))
                            {
                                int multIndex = (int)(multValue / 2);

                                // Because primeArray is initialized entirely to false, we set indices to true if they are NOT prime
                                primeArray[multIndex] = true;

                                // We use a temp variable anywhere that we need to increment, decrement, or perform a similar operation in order to keep things atomic
                                int temp = multValue;
                                multValue = temp + factor;

                                while (multValue % 2 == 0)
                                {
                                    temp = multValue;
                                    multValue = temp + factor;
                                }
                            }

                        }
                    }
                }
            );

            int count = 1;
            int j = 1;
            int temp2;
            int currentPrimeValue;

            while (j < _halfLimit)
            {
                if (!primeArray[j])
                {
                    currentPrimeValue = (int)(2 * (j + 0.5));
                    primeList.AddLast(currentPrimeValue);

                    temp2 = count;
                    count = temp2 + 1;
                }

                temp2 = j;
                j = temp2 + 1;
            }

            return primeList;
        }
    }

    class Solution
    {
        private const int UPPER_LIMIT = 100000000;
        private const int MAX_NUM_THREADS = 8;

        static void Main(String[] args)
        {
            DateTime startTime = DateTime.Now;

            PrimeSieveSolver solver = new PrimeSieveSolver(UPPER_LIMIT, MAX_NUM_THREADS);

            LinkedList<int> ansList = solver.FindPrimes();
            int[] maxPrimes = new int[10];

            int primesCount = ansList.Count;
            ulong totalSum = 0;
            ulong temp;
            int topTen = 0;

            // Processing the returned list of primes to get both the total sum of the primes as well as the list of the largest 10 in increasing order
            while (ansList.Count >= 1)
            {
                ulong curValue = (ulong)ansList.First.Value;
                temp = totalSum;
                totalSum = temp + curValue;

                if (ansList.Count <= 10)
                {
                    maxPrimes[topTen] = (int)curValue;
                    temp = (ulong)topTen;
                    topTen = (int)temp + 1;
                }

                ansList.RemoveFirst();
            }

            DateTime endTime = DateTime.Now;
            TimeSpan elapsedTime = endTime - startTime;

            WriteToFile(elapsedTime, primesCount, totalSum, maxPrimes);
        }

        static async Task WriteToFile(TimeSpan elapsedTime, int totalNumPrimes, ulong primeSum, int[] maxPrimes)
        {
            string[] lines =
            {
                elapsedTime.Seconds + " seconds, " + totalNumPrimes + " primes found, sum of primes = " + primeSum,
                maxPrimes[0] + ", " + maxPrimes[1] + ", " + maxPrimes[2] + ", " + maxPrimes[3] + ", " + maxPrimes[4] + ", " + maxPrimes[5] + ", " +
                maxPrimes[6] + ", " + maxPrimes[7] + ", " + maxPrimes[8] + ", " + maxPrimes[9]
             };

            await File.WriteAllLinesAsync("primes.txt", lines);
        }
    }
}


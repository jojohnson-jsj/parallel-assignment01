# Assignment01 - Primes

The core of my solution revolves around the parallelization of the Sieve of Eratosthenes. Initially, I used the brute force approach for determining whether a number is prime or not that I found here: https://stackoverflow.com/questions/15743192/check-if-number-is-prime-number

I then spawned 8 threads that would each receive as input a number from a shared Counter object. Accessing the counter was protected by a Mutex lock, and each thread was supposed to ultimately handle an equal portion of the load. However, this approach was taking 10+ minutes to run on my computer every time, so I began to research a more efficient way to determine whether a number is Prime or not. My old solution can be seen in the OldSolution folder.

This (https://stackoverflow.com/questions/17579091/faster-way-to-check-if-a-number-is-a-prime) post ultimately directed me towards the Sieve of Eratosthenes as a more viable solution for efficiently determining primes. I attempted to do a similar 8 thread input distribution across the bool array that I was using, but manually directing when each individual thread should .Join() and what the range of input for each thread should be proved to be inefficient. This solution got me down to a roughly 30 second execution time on my computer. 

My final solution, contained in the Assignment01 folder, uses the Sieve of Eratosthenes but makes a few improvements on my second one. A large one is that no room for even numbers is made in the bool array and that even numbers are not evaluated for whether or not they are prime. Another is that I'm using C#'s Parallel.For loop to handle a lot of the thread management more efficiently. This solution brought down my execution to roughly 2 seconds.

## In order to run this program:

On Linux:
1. In a Terminal window, navigate to the same directory where you downloaded Program.cs
2. Type:
4. sudo apt update
5. sudo apt install mono-complete
6. mcs -out:Program.exe Program.cs
7. mono Program.cs
7. The output file (primes.txt) will be located in the same directory where you placed Program.cs


On Visual Studio:
1. Open the Program.sln file located in the Assignment01 folder
2. Hit run
3. The primes.txt file will be in bin\Debug\netcoreapp3.1\primes.txt

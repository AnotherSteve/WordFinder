using System;
using System.IO;
using System.Linq;

namespace Dawg
{
    class Program
    {
        private readonly ScrabbleFinder.ScrabbleFinder _scrabbleFinder = new ScrabbleFinder.ScrabbleFinder();

        static void Main()
        {

            new Program();

        }

        public Program()
        {
            //Input = new String("?A???????????".ToCharArray().ToArray()); 

            //Stopwatch stopWatch = new Stopwatch();


            //stopWatch.Start();


            MakeThings(); 
            //stopWatch.Stop();
            //TimeSpan ts = stopWatch.Elapsed;
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            //       ts.Hours, ts.Minutes, ts.Seconds,
            //       ts.Milliseconds);
            //Console.WriteLine("RunTime " + elapsedTime);
            //stopWatch.Restart();
            string input = null;
            while (true)
            {
                var readLine = Console.ReadLine();
                if (readLine != null) input = readLine.ToUpper();
                if (input != null && !input.Contains('!'))
                {
                    DoThings(input);
                }
                else
                {
                    break;
                }
            }
            //stopWatch.Stop();
            //ts = stopWatch.Elapsed;
            //elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            //       ts.Hours, ts.Minutes, ts.Seconds,
            //       ts.Milliseconds);
            //Console.WriteLine("RunTime " + elapsedTime);

        }


        private void MakeThings()
        {
            using (var file = new System.IO.StreamReader(@"ScrabbleDict.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    _scrabbleFinder.AddWord(line);
                }
            }
        }

        private void DoThings(string input)
        {
            System.IO.File.WriteAllLines(input.Replace('?', 'w') + ".txt", _scrabbleFinder[input].OrderBy(a => a).OrderByDescending(a => a.Length));

            Console.WriteLine("Solutions Written to " + input.Replace('?', 'w') + ".txt");
        }
    }
}


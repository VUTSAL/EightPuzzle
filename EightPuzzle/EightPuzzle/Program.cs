using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EightPuzzle;

namespace EightPuzzle
{
    public class States
    {
        public int Depth { get; set; }
        public long SearchCostH1 { get; set; }
        public long TimeCostH1 { get; set; }
        public long SearchCostH2 { get; set; }
        public long TimeCostH2 { get; set; }
        public int TestCaseH1 { get; set; }
        public int TestCaseH2 { get; set; }
        
      
    }
    
    class Program
    {
       
        static void Main()
        {
    
           
            #region DataMembers
        
            var FinalConfig = new[,] {    {0,1,2},
                                             {3,4,5},
                                             {6,7,8}
                                             };
            var finalState = new StateNode<int>(FinalConfig, 0, 0, 0);
            var watch = new Stopwatch();

            long SearchCosth1 = 0, TimeCosth1 = 0, SearchCosth2 = 0, TimeCosth2 = 0;
          
            bool Exit = false;
            
            int answer = 0;
            int count = 0;
            int Heuristic = 0;
            
            string puzzle = string.Empty;
            
            List<States> StateList = new System.Collections.Generic.List<States>();
            #endregion 

            #region Puzzle
            try
            {
                while (!Exit)
                {
                    
                    Console.WriteLine("Choose option");
                    Console.WriteLine("0.Exit");
                    Console.WriteLine("1.Randomly generated puzzle");
                    Console.WriteLine("2.Enter Puzzle(Example:312645078)");
                    Console.WriteLine("3.Test Samples");
                    answer = Convert.ToInt32(Console.ReadLine());
               
                    #region RandomlyGeneratedPuzzle
                    if (answer == 1)
                    {

                        RandomlyGeneratedPuzzle(Heuristic, finalState, watch);
                        
                    }
                    #endregion
                 
                    #region Manual
                    else if (answer == 2)
                    {
                        ManualPuzzle(puzzle, Heuristic, finalState, watch);
                        
                    }
                    #endregion
                  
                    #region TestSamples
                    else if (answer == 3)
                    {
                        TestSamples(puzzle, finalState, watch,count,StateList,SearchCosth1,SearchCosth2,TimeCosth1,TimeCosth2);
                      
                    }
                    #endregion
                    else
                    {
                        Exit = true;
                    }

                }
            }
            catch (System.Exception EX)
            {
              
                Console.WriteLine("Something went wrong");
            }
            #endregion

        }

      

        #region PrivateMethods
        private static void Showsolution(StateNode<int> node)
        {
            if (node.Parent != null)
            {
                Showsolution(node.Parent);
            }

            node.Print();

        }

        private static void TestSamples(string puzzle, StateNode<int> finalState, Stopwatch watch, int count, System.Collections.Generic.List<States> StateList, long SearchCosth1, long SearchCosth2, long TimeCosth1, long TimeCosth2)
        {
            try
            {
                Console.WriteLine("Enter Path of the file(Ex:c://test.txt) ");
                string Path = Console.ReadLine();
                var reader = new StreamReader(Path != null && Path != string.Empty ? Path : "C://test.txt");
                
                bool StateAdded = false;
                Console.WriteLine(System.Environment.NewLine);

                

                Console.Write("Loading...");
                while (!reader.EndOfStream)
                {
                    Console.Write(".");
                    count++;
                    var line = reader.ReadLine();
                    if (line != null && line != string.Empty)
                    {
                        puzzle = line.ToString();
                        puzzle = String.Join<char>(" ", puzzle) + " ";
                        puzzle = puzzle.Trim();
                        int[][] resultTemp =
                                                    puzzle.Split(' ')
                                                    .Select((a, index) => new { index, value = int.Parse(a) })
                                                    .GroupBy(tuple => tuple.index / 3)
                                                    .Select(g => g.Select(tuple => tuple.value).ToArray())
                                                    .ToArray();
                        var result = new[,] { { resultTemp[0][0], resultTemp[0][1], resultTemp[0][2] }, { resultTemp[1][0], resultTemp[1][1], resultTemp[1][2] }, { resultTemp[2][0], resultTemp[2][1], resultTemp[2][2] } };
                        Tuple<int, int> EmptyRowCOlumn = CoordinatesOf(result, 0);

                        var initialState = new StateNode<int>(result, EmptyRowCOlumn.Item1, EmptyRowCOlumn.Item2, 0);



                        for (int l = 0; l < 2; l++)
                        {
                            Console.Write(".");
                            HeuristicTypes ht = l == 0 ? HeuristicTypes.MisplacedTiles : HeuristicTypes.ManhattanDistance;
                            var aStar = new AStar<int>(initialState, finalState, 0, ht);
                            watch.Start();
                            var node = aStar.Execute();
                            watch.Stop();
                            StateAdded = false;
                            if (node != null)
                            {
                                if (StateList != null && StateList.Count > 0)
                                {
                                    foreach (States st in StateList)
                                    {


                                        if (st.Depth == node.Depth)
                                        {
                                            StateAdded = true;

                                            if (ht == HeuristicTypes.MisplacedTiles)
                                            {

                                                st.SearchCostH1 += aStar.StatesVisited;
                                                st.TimeCostH1 += watch.ElapsedMilliseconds;
                                                st.TestCaseH1++;
                                            }
                                            else
                                            {
                                                st.SearchCostH2 += aStar.StatesVisited;
                                                st.TimeCostH2 += watch.ElapsedMilliseconds;
                                                st.TestCaseH2++;
                                            }

                                        }




                                    }

                                }
                                if (!StateAdded)
                                {
                                    States s = new States();
                                    s.Depth = node.Depth;

                                    if (ht == HeuristicTypes.MisplacedTiles)
                                    {

                                        s.SearchCostH1 += aStar.StatesVisited;
                                        s.TimeCostH1 += watch.ElapsedMilliseconds;
                                        s.TestCaseH1++;
                                    }
                                    else
                                    {
                                        s.SearchCostH2 += aStar.StatesVisited;
                                        s.TimeCostH2 += watch.ElapsedMilliseconds;
                                        s.TestCaseH2++;
                                    }

                                    StateList.Add(s);
                                }

                            }
                        }

                    }

                }
                Console.Write(".");
                reader.Close();
                if (StateList != null && StateList.Count > 0)
                {
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("Status:");
                    Console.WriteLine(System.Environment.NewLine);
                    Console.WriteLine("  Depth" + "  SearchCosth1" + "  TimeCosth1" + "  SearchCosth2" + "  TimeCosth2" + "  TestCasesh1" + "  TestCasesh2");
                    Console.WriteLine(System.Environment.NewLine);

                    List<States> SortedStates = StateList.OrderBy(t => t.Depth).ToList();
                    foreach (States s in SortedStates)
                    {
                        Console.WriteLine(System.Environment.NewLine);
                        SearchCosth1 = s.TestCaseH1 > 0 ? (s.SearchCostH1 / s.TestCaseH1) : 0;
                        TimeCosth1 = s.TestCaseH1 > 0 ? (s.TimeCostH1 / s.TestCaseH1) : 0;
                        SearchCosth2 = s.TestCaseH2 > 0 ? (s.SearchCostH2 / s.TestCaseH2) : 0;
                        TimeCosth2 = s.TestCaseH2 > 0 ? (s.TimeCostH2 / s.TestCaseH2) : 0;
                        Console.Write("{0,6} {1,13} {2,11} {3,13} {4,11} {5,12} {6,12} ", s.Depth.ToString(), SearchCosth1.ToString(), TimeCosth1.ToString(), SearchCosth2.ToString(), TimeCosth2.ToString(), s.TestCaseH1.ToString(), s.TestCaseH2.ToString());
                        Console.WriteLine(System.Environment.NewLine);
                    }
                }
            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }

        private static void ManualPuzzle(string puzzle, int Heuristic, StateNode<int> finalState, Stopwatch watch)
        {
            try
            {
                Console.WriteLine("Enter Puzzle");
                puzzle = Console.ReadLine();
                Heuristic = getHeuristic();

                puzzle = String.Join<char>(" ", puzzle) + " ";
                puzzle = puzzle.Trim();
                int[][] resultTemp =
                                            puzzle.Split(' ')
                                            .Select((a, index) => new { index, value = int.Parse(a) })
                                            .GroupBy(tuple => tuple.index / 3)
                                            .Select(g => g.Select(tuple => tuple.value).ToArray())
                                            .ToArray();
                var result = new[,] { { resultTemp[0][0], resultTemp[0][1], resultTemp[0][2] }, { resultTemp[1][0], resultTemp[1][1], resultTemp[1][2] }, { resultTemp[2][0], resultTemp[2][1], resultTemp[2][2] } };
                Tuple<int, int> EmptyRowCOlumn = CoordinatesOf(result, 0);

                var initialState = new StateNode<int>(result, EmptyRowCOlumn.Item1, EmptyRowCOlumn.Item2, 0);

                var aStar = new AStar<int>(initialState, finalState, 0);

                watch.Start();
                var node = aStar.Execute();
                watch.Stop();

                if (node != null)
                {

                    Showsolution(node);
                    Console.WriteLine("Depth Level {0}", node.Depth);
                    Console.WriteLine("Nodes generated {0}", aStar.StatesVisited);
                }
                else
                {
                    Console.WriteLine("No Solution Found");
                }
                Console.WriteLine("Elapsed miliseconds {0} ", watch.ElapsedMilliseconds);
            }
            catch (System.Exception ex)
            {

                throw ex;
            }

        }

        private static void RandomlyGeneratedPuzzle(int Heuristic, StateNode<int> finalState, Stopwatch watch)
        {
            try
            {
               LinearShuffle<int> shuffle = new LinearShuffle<int>();
               int[] defaultConfig = new int[] { 8, 7, 2, 4, 6, 3, 1, 0, 5 };
               shuffle.Shuffle(defaultConfig);
                
                Heuristic = getHeuristic();
                var result = new[,] { { defaultConfig[0], defaultConfig[1], defaultConfig[2] }, { defaultConfig[3], defaultConfig[4], defaultConfig[5] }, { defaultConfig[6], defaultConfig[7], defaultConfig[8] } };
                Tuple<int, int> EmptyRowCOlumn = CoordinatesOf(result, 0);

                var initialState = new StateNode<int>(result, EmptyRowCOlumn.Item1, EmptyRowCOlumn.Item2, 0);


                var aStar = new AStar<int>(initialState, finalState, 0, Heuristic == 1 ? HeuristicTypes.MisplacedTiles : HeuristicTypes.ManhattanDistance);
                watch.Start();
                var node = aStar.Execute();
                watch.Stop();

                if (node != null)
                {

                    Showsolution(node);
                    Console.WriteLine("Depth Level {0}", node.Depth);
                    Console.WriteLine("Nodes generated {0}", aStar.StatesVisited);
                }
                else
                {
                    Console.Write(Environment.NewLine + Environment.NewLine);
                    int rowLength = result.GetLength(0);
                    int colLength = result.GetLength(1);

                    for (int i = 0; i < rowLength; i++)
                    {
                        for (int j = 0; j < colLength; j++)
                        {
                            Console.Write(string.Format("{0} ", result[i, j]));
                        }
                        Console.Write(Environment.NewLine + Environment.NewLine);
                    }
                    Console.WriteLine("No Solution Found");
                }
                Console.WriteLine("Elapsed miliseconds {0}", watch.ElapsedMilliseconds);

            }
            catch (System.Exception ex)
            {

                throw ex;
            }
        }
        internal sealed class LinearShuffle<T>
        {
            #region Fields

            private Random lRandom;

            #endregion Fields

            #region Methods
            internal LinearShuffle()
            {
                int seed = 37 + 37 * ((int)DateTime.Now.TimeOfDay.TotalSeconds % 37);
                lRandom = new Random(seed);
            }

            internal void Shuffle(T[] array)
            {
                int position;
                for (int i = 0; i < array.Length; i++)
                {
                    position = NextRandom(0, i);
                    Swap(array, i, position);
                }
            }

            private int NextRandom(int min, int max)
            {
                return lRandom.Next(min, max);
            }

            private void Swap(T[] a, int i, int j)
            {
                T temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }

            #endregion Methods
        }
        private static int getHeuristic()
        {
            try
            {
                Console.WriteLine("1.MisplacedTiles");
                Console.WriteLine("2.ManhattanDistance");
                return Convert.ToInt32(Console.ReadLine());
            }
            catch (System.Exception ex)
            {

                return 1;//default -misplaced tiles
            }
        }
        #endregion 

        #region Public Methods
        public static Tuple<int, int> CoordinatesOf(int[,] matrix, int value)
        {
            int w = matrix.GetLength(0); // width
            int h = matrix.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (matrix[x, y].Equals(value))
                        return Tuple.Create(x, y);
                }
            }

            return Tuple.Create(-1, -1);
        }
        #endregion
    }
}

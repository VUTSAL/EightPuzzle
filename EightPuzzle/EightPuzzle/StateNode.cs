using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EightPuzzle
{
  

       public class StateNode<T> : IComparable<StateNode<T>> where T : IComparable
        {
            public double Value { get; set; }//F=G+H
            public T[,] State { get; private set; }//State represented by this node
            public int EmptyCol { get; private set; }//column number where the empty tile is located 
            public int EmptyRow { get; private set; }//row number where the empty tile is located 
            public int Depth { get; set; }//depth of this state from initial state
            public string StrRepresentation { get; set; }
            public string Path { get; set; }

            public StateNode<T> Parent { get; set; }
            public List<StateNode<T>> ChildNodes { get; set; }
            public StateNode() { ChildNodes = new List<StateNode<T>>(); }

            public StateNode(T[,] state, int emptyRow, int emptyCol, int depth)
            {
                ChildNodes = new List<StateNode<T>>();
                if (state.GetLength(0) != state.GetLength(1))
                    throw new Exception("Number of columns and rows must be the same");

                State = state.Clone() as T[,];
                EmptyRow = emptyRow;
                EmptyCol = emptyCol;
                Depth = depth;

                for (var i = 0; i < State.GetLength(0); i++)
                {
                    for (var j = 0; j < State.GetLength(1); j++)
                        StrRepresentation += State[i, j] + ",";
                }
            }

            public int Size
            {
                get { return State.GetLength(0); }
            }

            public void Print()
            {
                for (var i = 0; i < State.GetLength(0); i++)
                {
                    for (var j = 0; j < State.GetLength(1); j++)
                        Console.Write(State[i, j] + " ");
                    Console.WriteLine();
                }
                Console.WriteLine();
            }

            public int CompareTo(StateNode<T> other)
            {
                if (Value > other.Value)
                    return 1;
                if (Value < other.Value)
                    return -1;

                return 0;
            }
        }
   
}

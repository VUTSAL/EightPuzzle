using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EightPuzzle;



namespace EightPuzzle
{
    public enum HeuristicTypes
    {
        MisplacedTiles = 1,
        ManhattanDistance = 2

    }
    class AStar<T> where T : IComparable
    {
     
        public int StatesVisited { get; set; }
        public HeuristicTypes _hType { get; set; }
        public Dictionary<string, int> PatternDatabase { get; set; }

        private readonly StateNode<T> _goal;//goal state node
        public T Empty { get; set; }//Empty tile
        private readonly PriorityQueue<StateNode<T>> _queue;
        private readonly HashSet<string> _hash;

        public AStar(StateNode<T> initial, StateNode<T> goal, T empty, HeuristicTypes HType=HeuristicTypes.ManhattanDistance)
        {
            _queue = new PriorityQueue<StateNode<T>>(new[] { initial });
            _goal = goal;
            Empty = empty;
            _hash = new HashSet<string>();
            _hType = HType;
        }

        public StateNode<T> Execute()
        {

            _hash.Add(_queue.Min().StrRepresentation);

            while (_queue.Count > 0)
            {
                var current = _queue.Pop();
                StatesVisited++;
                if (current.StrRepresentation.Equals(_goal.StrRepresentation))
                    return current;

                ExpandNodes(current);

            }

            return null;
        }

        private void ExpandNodes(StateNode<T> node)
        {
            T temp;
            T[,] newState;
            var col = node.EmptyCol;
            var row = node.EmptyRow;
            StateNode<T> newNode;

            // Up
            if (row > 0)
            {
                newState = node.State.Clone() as T[,];
                temp = newState[row - 1, col];
                newState[row - 1, col] = Empty;
                newState[row, col] = temp;
                newNode = new StateNode<T>(newState, row - 1, col, node.Depth + 1);

                if (!_hash.Contains(newNode.StrRepresentation))
                {
                    newNode.Value = node.Depth + Heuristic(newNode);
                    newNode.Path = node.Path + "U";
                    newNode.Parent = node;
                    node.ChildNodes.Add(newNode);
                    _queue.Push(newNode);
                    _hash.Add(newNode.StrRepresentation);
                }
            }

            // Down
            if (row < node.Size - 1)
            {
                newState = node.State.Clone() as T[,];
                temp = newState[row + 1, col];
                newState[row + 1, col] = Empty;
                newState[row, col] = temp;
                newNode = new StateNode<T>(newState, row + 1, col, node.Depth + 1);

                if (!_hash.Contains(newNode.StrRepresentation))
                {
                    newNode.Value = node.Depth + Heuristic(newNode);
                    newNode.Path = node.Path + "D";
                    newNode.Parent = node;
                    node.ChildNodes.Add(newNode);
                    _queue.Push(newNode);
                    _hash.Add(newNode.StrRepresentation);
                }
            }

            // Left
            if (col > 0)
            {
                newState = node.State.Clone() as T[,];
                temp = newState[row, col - 1];
                newState[row, col - 1] = Empty;
                newState[row, col] = temp;
                newNode = new StateNode<T>(newState, row, col - 1, node.Depth + 1);

                if (!_hash.Contains(newNode.StrRepresentation))
                {
                    newNode.Value = node.Depth + Heuristic(newNode);
                    newNode.Path = node.Path + "L";
                    newNode.Parent = node;
                    node.ChildNodes.Add(newNode);
                    _queue.Push(newNode);
                    _hash.Add(newNode.StrRepresentation);
                }
            }

            // Right
            if (col < node.Size - 1)
            {
                newState = node.State.Clone() as T[,];
                temp = newState[row, col + 1];
                newState[row, col + 1] = Empty;
                newState[row, col] = temp;
                newNode = new StateNode<T>(newState, row, col + 1, node.Depth + 1);

                if (!_hash.Contains(newNode.StrRepresentation))
                {
                    newNode.Value = node.Depth + Heuristic(newNode);
                    newNode.Path = node.Path + "R";
                    newNode.Parent = node;
                    node.ChildNodes.Add(newNode);
                    _queue.Push(newNode);
                    _hash.Add(newNode.StrRepresentation);
                }
            }
        }


        private double Heuristic(StateNode<T> node)
        {
            if (_hType == HeuristicTypes.ManhattanDistance)
            {

                return ManhattanDistance(node);
            }
            else
            {
                return MisplacedTiles(node);
            }
            
        }

        private int MisplacedTiles(StateNode<T> node)
        {
            var result = 0;

            for (var i = 0; i < node.State.GetLength(0); i++)
            {
                for (var j = 0; j < node.State.GetLength(1); j++)
                    if (!node.State[i, j].Equals(_goal.State[i, j]) && !node.State[i, j].Equals(Empty))
                        result++;
            }

            return result;
        }

        private int ManhattanDistance(StateNode<T> node)
        {
            var result = 0;

            for (var i = 0; i < node.State.GetLength(0); i++)
            {
                for (var j = 0; j < node.State.GetLength(1); j++)
                {
                    var elem = node.State[i, j];
                    if (elem.Equals(Empty)) continue;
                    
                    var found = false;
                    for (var h = 0; h < _goal.State.GetLength(0); h++)
                    {
                        for (var k = 0; k < _goal.State.GetLength(1); k++)
                        {
                            if (_goal.State[h, k].Equals(elem))
                            {
                                result += Math.Abs(h - i) + Math.Abs(j - k);
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                }
            }

            return result;
        }
    

    }
}

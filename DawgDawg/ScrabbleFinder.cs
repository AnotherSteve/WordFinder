using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScrabbleFinder
{
    [Serializable]
    public class ScrabbleFinder : LetterNode
    {

        public ScrabbleFinder() : base() 
        {
          
        }

        public void AddWord(String word)
        {
            var input = word.ToCharArray();
            Array.Sort(input);
            AddWord(input, word);
        }

        private HashSet<String> IsMatch(string word)
        {
            return IsMatch(word.ToCharArray());
        }

        private new HashSet<String> IsMatch(Char[] word)
        {
            if (Outs.ContainsKey(word[0]))
            {
                return Outs[word[0]].IsMatch(word);
            }
            else if (word[0] == Wildcard)
            {
                var outs = new HashSet<string>(Words);
                foreach (var wrd in Outs.Values.SelectMany(child => child.IsMatch(Next(word))))
                {
                    outs.Add(wrd);
                }
                return outs;
            }
            else return new HashSet<String>();
        }

        public HashSet<string> this[string key]
        {
            get
            {
                var solutions = new HashSet<String>();
                var checkedPatterns = new HashSet<string>();
                return MakeSubsets(key.ToCharArray(), solutions, checkedPatterns);
            }
        }

        private void FindResults(char[] solution, ISet<string> solutions)
        {
            if (solution.Contains(Wildcard))
            {
                //Put the wilds in the front
                Array.Sort(solution);
                for (var wildnext = solution; wildnext != null; wildnext = NextWildPermutation(wildnext))
                {
                    foreach (var word in IsMatch(wildnext))
                    {

                        solutions.Add(word);
                    }
                }
                return;
            }
            foreach (var word in IsMatch(solution))
            {
                solutions.Add(word);
            }
        }


        //private bool IsSubset(string subset)
        //{
        //    Char[] orderedSubset = subset.ToCharArray().OrderBy(a => a).ToArray();
        //    int y = Input.IndexOf(orderedSubset[0]);
        //    if (y < 0) return false;
        //    bool result = false;
        //    for (int i = 0; i < subset.Length; )
        //    {
        //        if (y == Input.Length) return false;
        //        result = orderedSubset[i] == Input[y];
        //        if (result) i++;
        //        y++;
        //    }
        //    return result;
        //}


        private char[] OrderForWildCard(IList<char> set)
        {
            if (!set.Contains(Wildcard)) return set.OrderBy(a => a).ToArray();
            var wildcardSpots = new List<int>();
            for (var i = 0; i < set.Count; i++)
            {
                if (Wildcard == set[i]) wildcardSpots.Add(i);
            }
            var noWildSet = set.Where(a => a != Wildcard).OrderBy(a => a).ToArray();
            Array.Resize(ref noWildSet, noWildSet.Length + wildcardSpots.Count);
            foreach (var wildSpot in wildcardSpots)
            {
                for (var i = 1; i < noWildSet.Length - wildSpot; i++)
                {
                    noWildSet[noWildSet.Length - i] = noWildSet[noWildSet.Length - i - 1];
                }
                noWildSet[wildSpot] = Wildcard;
            }
            return noWildSet;
        }

        private HashSet<String> MakeSubsets(char[] set, HashSet<String> solutions, HashSet<String> checks)
        {
            var testSet = OrderForWildCard(set);
            var setString = new String(testSet);
            if (checks.Contains(setString)) return solutions;
            checks.Add(setString);
            FindResults(testSet, solutions);
            //could rotate the array instead of this
            //char[] doubleSet = new char[set.Length * 2];
            //Array.Copy(set, 0, doubleSet, 0, set.Length);
            //Array.Copy(set, 0, doubleSet, set.Length, set.Length);
            for (var i = 1; i < set.Length + 1; i++)
            {
                var next = new char[set.Length - 1];
                RotateArrayRight(set, 1);
                Array.Copy(set, 0, next, 0, next.Length);
                if (next.Length > 1)
                {
                    MakeSubsets(next, solutions, checks);
                 }
            }
            return solutions;
        }

        private char[] NextWildPermutation(char[] set)
        {
            var wildCount = 0;
            Array.ForEach(set, (a) => { if (a == Wildcard) wildCount++; });
            var wildPlaces = new int[wildCount];
            var u = 0;
            for (var i = 0; i < set.Length; i++)
            {
                if (set[i] != Wildcard) continue;
                wildPlaces[u] = i;
                u++;
            }
            Array.Reverse(wildPlaces);
            if (wildPlaces[0] != set.Length - 1)
            {
                Swap(ref set[wildPlaces[0]], ref set[wildPlaces[0] + 1]);
                return set;
            }
            /*
            * 
            * Move next wild place that does not have a wild to the right to next non-wild to it's right
            * then move all wilds to the right to it's immediate right
            *
            */

            //get next wild place that does not have a wild to the right
            var nextWild = -1;
            var endWilds = 1;
            for (var i = 1; i < wildPlaces.Length; i++)
            {
                if (wildPlaces[i] != wildPlaces[i - 1] - 1)
                {
                    nextWild = wildPlaces[i];
                    break;
                }
                else endWilds++;
            }
            if (endWilds == wildPlaces.Length) return null; //we're done
            //move next wild one to the right
            Swap(ref set[nextWild], ref set[nextWild + 1]);
            nextWild++; //move the pointer to it over
            //move all wilds to the right next to it
            var rotate = new char[set.Length - nextWild - 1];
            Array.Copy(set, nextWild + 1, rotate, 0, rotate.Length);
            RotateArrayRight(rotate, endWilds);
            Array.Copy(rotate, 0, set, nextWild + 1, rotate.Length);
            return set;
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            var swap = a;
            a = b;
            b = swap;
        }

        private static void RotateArrayRight(char[] array, int times)
        {
            var moveArray = new char[array.Length + 1];
            Array.Copy(array, moveArray, array.Length);
            for (var m = 0; m < times; m++)
            {
                for (var i = 1; i < moveArray.Length + 1; i++)
                {
                    if (i == moveArray.Length)
                        Swap(ref moveArray[moveArray.Length - 1], ref moveArray[0]);
                    else
                    {
                        Swap(ref moveArray[moveArray.Length - i - 1], ref moveArray[moveArray.Length - i]);
                    }
                }
            }
            Array.Copy(moveArray, 0, array, 0, array.Length);
        }
    }
}

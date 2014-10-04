using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ScrabbleFinder
{
    [Serializable]
    public class LetterNode
    {
        protected readonly Char Wildcard = '?'; 
        public Char Letter { get; set; }
        protected Dictionary<Char, LetterNode> Outs { get; set; }
        public Boolean EndNode { get; set; }
        public HashSet<String> Words { get; set; }

        public LetterNode(Char letter) : this()
        {
            this.Letter = letter;
            
        }

        public LetterNode()
        {
            EndNode = false;
            Outs = new Dictionary<char, LetterNode>();
            Words = new HashSet<String>();
        }

        protected void AddLetter(Char letter)
        {
            if (Outs.ContainsKey(letter)) return;
            Outs.Add(letter, new LetterNode(letter)); 
        }

        protected void AddWord(Char[] orderedWord, String word)
        {
            AddLetter(orderedWord[0]);
            if (orderedWord.Length > 1)
            {

                Outs[orderedWord[0]].AddWord(Next(orderedWord), word);
            }
            else
            {
                Outs[orderedWord[0]].EndNode = true;
                Outs[orderedWord[0]].Words.Add(word);
            }
        }

        internal HashSet<String> IsMatch(Char[] word)
        {
           
            if (word.Length == 1)
            {
                if ((word[0] == Letter || word[0] == Wildcard) && EndNode) return Words;
                else return new HashSet<String>();
            }
            else if (Outs.ContainsKey(word[1]))
            {
                return GetOutWords(Outs[word[1]], word);
            }
            else if (word[1] == Wildcard)
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

        private HashSet<string> GetOutWords(LetterNode Out, Char[] word)
        {
            var outs = Out.IsMatch(Next(word));

            if (outs.Count <= 0) return outs;
            Monitor.Enter(Words);
            foreach (var w in Words)
            {
                outs.Add(w);
            }
            Monitor.Exit(Words);
            return outs;
        }

        protected LetterNode GetMatchBigNode(Char[] word, int depth, int level)
        {
            if (depth == level && word[0] == Letter) return this;
            return Outs.ContainsKey(word[1]) ? Outs[word[1]].GetMatchBigNode(Next(word), ++depth, level) : null;
        }

        protected Char[] Next(Char[] word)
        {
            var next = new Char[word.Length - 1];
            Array.Copy(word, 1, next, 0, next.Length);
            return next;
        }


    }
}

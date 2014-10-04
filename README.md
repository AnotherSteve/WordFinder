WordFinder
==========

There's a little console app that runs the wordfinder.

It finds all the scrabble moves possible for a set of letters with wildcards. 

Using a scrabble dictionary it makes a tree with letters on the nodes by alphabatizing the words and drop them into the tree
letter by letter. Nodes that are word ends are marked, all leaves are the ends of words.

If a combination of an input string can follow a path in the tree it's a legal scrabble word.

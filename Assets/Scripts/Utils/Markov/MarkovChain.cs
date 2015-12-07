using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Linq;
using System.Text.RegularExpressions;


public class MarkovChainGenerator {
	public static string Generate (string str) { // string[] args
		// sample data set
		string seed = str; //Tidy(str);

		//Debug.Log(">>> " + seed);

		List<string> generated = GenerateChainList(seed);
		Debug.Log(generated.Count);

		string output = "";
		foreach(string item in generated) {
			output += item;
			
		}

		Debug.Log(output);
		return output;
	}


	private static List<string> GenerateChainList (string seed) { // string[] args
		// tokenise the input string
		var seedList = new List<string>(Split(seed.ToLower()));
		// create a chain with a window size of 4
		var chain = new Chain<string>(seedList, 4);

		// generate a new sequence using a starting word, and maximum return size
		var generated = new List<string>(chain.Generate("twinkle", 2000));
		// output the results to the console
		generated.ForEach(item => Console.Write("{0}", item));

		return generated;
	}

	// tokenise a string into words (regex definition of word)
	private static IEnumerable<string> Split(string subject) {
		List<string> tokens = new List<string>();
		Regex regex = new Regex(@"(\W+)");
		tokens.AddRange(regex.Split(subject));

		return tokens;
	}


	private static string Tidy(string p) {
		string result = p.Replace('\t', ' ');
		string compress = result;

		do {
			result = compress;
			compress = result.Replace(" ", " ");
		} while (result != compress);
		
		return result;
	}

}



/* A markov chain: */


/// <summary>
 /// Chain implements a markov chain for a type T
 /// allows the generation of sequences based on
 /// a sample set of T items
 /// </summary>
 /// <typeparam name="T">the type of elements</typeparam>
 public class Chain<T>
 {
  Link<T> root = new Link<T>(default(T));
  int length;

  /// <summary>
  /// creates a new chain
  /// </summary>
  /// <param name="input">Sample set</param>
  /// <param name="length">window size for sequences</param>
  public Chain(IEnumerable<T> input, int length)
  {
   this.length = length;
   root.Process(input, length);
  }

  /// <summary>
  /// generate a new sequence based on the samples first entry
  /// </summary>
  /// <param name="max">maximum size of result</param>
  /// <returns></returns>
  public IEnumerable<T> Generate(int max)
  {
   foreach (Link<T> next in root.Generate(root.SelectRandomLink().Data, length, max))
    yield return next.Data;
  }

  /// <summary>
  /// generate a new sequence based on the sample
  /// </summary>
  /// <param name="start">the item to start with</param>
  /// <param name="max">maximum size of result</param>
  /// <returns></returns>
  public IEnumerable<T> Generate(T start, int max)
  {
   foreach (Link<T> next in root.Generate(start, length, max))
    yield return next.Data;
  }
 }


/* consists of links: */

 /// <summary>
 /// parts of a chain (markcov)
 /// </summary>
 /// <typeparam name="T">link type</typeparam>
 internal class Link<T>
 {
  T data;
  int count;
  // following links
  Dictionary<T, Link<T>> links;

  private Link()
  {
  }

  /// <summary>
  /// create a new link
  /// </summary>
  /// <param name="data">value of the item in sequence</param>
  internal Link(T data)
  {
   this.data = data;
   this.count = 0;

   links = new Dictionary<T, Link<T>>();
  }

  /// <summary>
  /// process the input in window sized chunks
  /// </summary>
  /// <param name="input">the sample set</param>
  /// <param name="length">size of sequence window</param>
  public void Process(IEnumerable<T> input, int length)
  {
   // holds the current window
   Queue<T> window = new Queue<T>(length);

   // process the input, a window at a time (overlapping)
   foreach (T part in input)
   {
    if (window.Count == length)
     window.Dequeue();
    window.Enqueue(part);

    ProcessWindow(window);
   }
  }

  /// <summary>
  /// process the window to construct the chain
  /// </summary>
  /// <param name="window"></param>
  private void ProcessWindow(Queue<T> window)
  {
   Link<T> link = this;

   foreach (T part in window)
    link = link.Process(part);
  }

  /// <summary>
  /// process an item following us
  /// keep track of how many times
  /// we are followed by each item
  /// </summary>
  /// <param name="part"></param>
  /// <returns></returns>
  internal Link<T> Process(T part)
  {
   Link<T> link = Find(part);

   // not been followed by this
   // item before
   if (link == null)
   {
    link = new Link<T>(part);
    links.Add(part, link);
   }

   link.Seen();

   return link;
  }

  private void Seen()
  {
   count++;
  }

  public T Data
  {
   get
   {
    return data;
   }
  }

  public int Occurances
  {
   get
   {
    return count;
   }
  }

  /// <summary>
  /// Total number of incidences after this link
  /// </summary>
  public int ChildOccurances
  {
   get
   {
    // sum all followers occurances
    int result = links.Sum(link => link.Value.Occurances);

    return result;
   }
  }

  public override string ToString()
  {
   return String.Format("{0} ({1})", data, count);
  }

  /// <summary>
  /// find a follower of this link
  /// </summary>
  /// <param name="start">item to be found</param>
  /// <returns></returns>
  internal Link<T> Find(T follower)
  {
   Link<T> link = null;

   if (links.ContainsKey(follower))
    link = links[follower];

   return link;
  }

  static System.Random rand = new System.Random();
  /// <summary>
  /// select a random follower weighted
  /// towards followers that followed us
  /// more often in the sample set
  /// </summary>
  /// <returns></returns>
  public Link<T> SelectRandomLink()
  {
   Link<T> link = null;

   int universe = this.ChildOccurances;

   // select a random probability
   int rnd = rand.Next(1, universe+1);

   // match the probability by treating
   // the followers as bands of probability
   int total = 0;
   foreach (Link<T> child in links.Values)
   {
    total += child.Occurances;

    if (total >= rnd)
    {
     link = child;
     break;
    }
   }

   return link;
  }

  /// <summary>
  /// find a window of followers that
  /// are after this link, returns where
  /// the last link if found, or null if
  /// this window never occured after this link
  /// </summary>
  /// <param name="window">the sequence to look for</param>
  /// <returns></returns>
  private Link<T> Find(Queue<T> window)
  {
   Link<T> link = this;

   foreach (T part in window)
   {
    link = link.Find(part);

    if (link == null)
     break;
   }

   return link;
  }

  /// <summary>
  /// a generated set of followers based
  /// on the likelyhood of sequence steps
  /// seen in the sample data
  /// </summary>
  /// <param name="start">a seed value to start the sequence with</param>
  /// <param name="length">how bug a window to use for sequence steps</param>
  /// <param name="max">maximum size of the set produced</param>
  /// <returns></returns>
  internal IEnumerable<Link<T>> Generate(T start, int length, int max)
  {
   var window = new Queue<T>(length);

   window.Enqueue(start);

   for (Link<T> link = Find(window); link != null && max != 0; link = Find(window), max--)
   {
    var next = link.SelectRandomLink();

    yield return link;

    if (window.Count == length-1)
     window.Dequeue();
    if (next != null)
     window.Enqueue(next.Data);
   }
  }
 }

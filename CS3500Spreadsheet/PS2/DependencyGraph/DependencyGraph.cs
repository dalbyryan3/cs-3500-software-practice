// DependencyGraph implementation written by Ryan Dalby u0848407

// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private int size;
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            size = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get
            {
                return size;
            }
        }


        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                if (this.HasDependees(s)) // if s has dependees
                {
                    return dependees[s].Count; // return number of dependees
                }
                else // s has no dependees
                {
                    return 0;
                }
            }
        }


        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            return dependents.ContainsKey(s); //If dependents does not contain s as a key then there are no values associated with it (these values would be the dependents of s)
        }


        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            return dependees.ContainsKey(s); //If dependees does not contain s as a key then there are no values associated with it (these values would be the dependees of s)
        }


        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if (this.HasDependents(s))
            {
                HashSet<string> enumerableDependents = dependents[s]; // Enumerate dependents of s since they exist
                return enumerableDependents;
            }
            else // This means there are no dependents of s
            {
                return new HashSet<string>(); // Return an empty HashSet<string>
            }
            
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (this.HasDependees(s))
            {
                HashSet<string> enumerableDependees = dependees[s]; // Enumerate dependees of s since they exist
                return enumerableDependees;
            }
            else // This means there are no dependees of s
            {
                return new HashSet<string>(); // Return an empty HashSet<string>
            }
        }


        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            if (!(this.HasDependents(s) && dependents[s].Contains(t))) // Checks if (s,t) does not exists currently in the DependencyGraph, if it does not exist will add it, otherwise will do nothing.
            {
                // Add dependent relationship for (s,t)
                if (this.HasDependents(s)) // This means we do not need a new dependents entry, just need to add t to the set
                {
                    dependents[s].Add(t); // Add t to the set that s maps to
                }
                else // This means we do need a new dependents entry and a set to add t to
                {
                    HashSet<string> tempSet = new HashSet<string>();
                    tempSet.Add(t);
                    dependents.Add(s, tempSet);
                }
                
                // Add dependee relationship for (s,t)
                if (this.HasDependees(t)) // This means we do not need a new dependee entry, just need to add s to the set
                {
                    dependees[t].Add(s); // Add s to the set that t maps to
                }
                else // This means we do need a new dependee entry and a set to add s to
                {
                    HashSet<string> tempSet = new HashSet<string>();
                    tempSet.Add(s);
                    dependees.Add(t, tempSet); // Add a dictionary entry for t mapping to a set that will initially contain s
                }

                size++; //Increase size of our DependencyGraph
            }
        }


        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            if (this.HasDependents(s) && dependents[s].Contains(t)) //Checks if (s,t) exists, if it does we can remove from dependents and dependees
            {
                //Remove from dependents
                if (dependents[s].Count == 1) // If we only have the single dependent t of s then the dependents entry s will be removed (thus also removing t from set)
                {
                    dependents.Remove(s);
                }
                else // Implies we have more than 1 dependent for s and thus will just remove t from the set
                {
                    dependents[s].Remove(t);
                }

                //Remove from dependees
                if (dependees[t].Count == 1) // If we only have the single dependee s of t then the dependents entry t will be removed (thus also removing s from set)
                {
                    dependees.Remove(t);
                }
                else // Implies we have more than 1 dependee for t and thus will just remove s from the set
                {
                    dependees[t].Remove(s);
                }
                size--; //Decreases size of our DependencyGraph
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (this.HasDependents(s)) // If s has dependents
            {
                List<string> rList = dependents[s].ToList<string>(); // r's to be removed 
                foreach (string r in rList) // Remove each (s,r)
                {
                    this.RemoveDependency(s, r);
                }
            }

            foreach (string t in newDependents) // Add each new ordered pair (s,t)
            {
                this.AddDependency(s, t);
            }
        }


        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (this.HasDependees(s)) // If s has dependees
            {
                List<string> rList = dependees[s].ToList<string>(); // r's to be removed 
                foreach (string r in rList) // Remove each (r,s)
                {
                    this.RemoveDependency(r, s);
                }
            }

            foreach (string t in newDependees) // Add each new ordered pair (t,s)
            {
                this.AddDependency(t, s);
            }
        }

    }

}


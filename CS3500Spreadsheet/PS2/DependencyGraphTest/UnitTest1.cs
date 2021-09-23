// DependencyGraphTest written by Ryan Dalby u0848407 with the inclusion of DependencyGraphTest tests from PS2skeleton 

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;

namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {
        /// <summary>
        /// Tests indexing using [] 
        ///</summary>
        [TestMethod()]
        public void Test1()
        {
            DependencyGraph t = new DependencyGraph();
            t.ReplaceDependees("B1", new List<string>() { "A1" });
            t.ReplaceDependees("A1", new HashSet<string>());
            IEnumerable<string> c = t.GetDependents("A1");
            
        }

        /// <summary>
        /// Tests indexing using [] 
        ///</summary>
        [TestMethod()]
        public void IndexerTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t["y"]);
            t.AddDependency("z", "y");
            Assert.AreEqual(2, t["y"]);
        }

        /// <summary>
        /// Tests indexing using [] with an empty DG
        ///</summary>
        [TestMethod()]
        public void IndexerEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t["y"]);
            t.AddDependency("x", "y");
            Assert.AreEqual(0, t["x"]);
        }

        /// <summary>
        /// Tests removing non-existent dependencies
        /// Tests removing existent dependencies
        ///</summary>
        [TestMethod()]
        public void SimpleRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
            t.RemoveDependency("x", "y");
            t.RemoveDependency("a", "z");
            Assert.AreEqual(4, t.Size);
            t.RemoveDependency("c", "b");
            Assert.AreEqual(3, t.Size);
            Assert.IsFalse(t.HasDependents("c"));
            t.RemoveDependency("a", "b");
            t.RemoveDependency("b", "d");
            Assert.IsTrue(t.HasDependents("a"));
            IEnumerator<string> e = t.GetDependents("a").GetEnumerator();
            e.MoveNext();
            Assert.AreEqual("c", e.Current);
        }

        /// <summary>
        /// Test adding the same pair to assure nothing happens
        ///</summary>
        [TestMethod()]
        public void AddDuplicatesTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            t.AddDependency("a", "b");
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        /// Test removing the same pair to assure nothing happens
        ///</summary>
        [TestMethod()]
        public void RemoveDuplicatesTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("a", "b");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("a", "b");
            t.RemoveDependency("a", "b");
            t.RemoveDependency("a", "b");
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        /// Tests has Dependents explicitly
        ///</summary>
        [TestMethod()]
        public void HasDependentsTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "d");
            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsFalse(t.HasDependents("d"));
        }

        /// <summary>
        /// Tests has Dependents explicitly
        ///</summary>
        [TestMethod()]
        public void HasDependeesTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("b", "d");
            Assert.IsTrue(t.HasDependees("b"));
            Assert.IsFalse(t.HasDependees("a"));
        }

        /// <summary>
        /// Test replace dependents
        ///</summary>
        [TestMethod()]
        public void ReplaceDependentsTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("a", "e");
            Assert.AreEqual(4, t.Size);
            HashSet<string> items = new HashSet<string>(new string[] { "f", "g" });
            t.ReplaceDependents("a", items);
            Assert.AreEqual(2, t.Size);
            t.AddDependency("b", "c");
            t.ReplaceDependents("a", new HashSet<string>());
            Assert.AreEqual(1, t.Size);
        }

        /// <summary>
        /// Test replace dependees
        ///</summary>
        [TestMethod()]
        public void ReplaceDependeesTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("a", "d");
            t.AddDependency("a", "e");
            Assert.AreEqual(4, t.Size);
            HashSet<string> items = new HashSet<string>(new string[] { "a" });
            t.ReplaceDependees("b", items);
            Assert.AreEqual(4, t.Size);
            t.AddDependency("c", "b");
            t.AddDependency("d", "b");
            t.AddDependency("e", "b");
            Assert.AreEqual(7, t.Size);
            t.ReplaceDependees("b", new HashSet<string>());
            Assert.AreEqual(3, t.Size);
            items = new HashSet<string>(new string[] { "a" });
            t.ReplaceDependees("b", items);
            Assert.AreEqual(4, t.Size);
        }

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }



        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }




        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }



        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }

    }
}

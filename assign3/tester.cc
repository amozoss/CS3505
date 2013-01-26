/*
 * This is a tester similar to the tester written in class.  It reads
 * words from a text file, then adds the words to two sets: A built-in
 * set class, and our utah_set class.  After reading the file, it
 * prints out all the words stored in the STL set object.  At the end
 * of the test, it prints out the sizes of both sets to see that they
 * are the same.
 *
 * After the test completes, I make sure the local variabls are properly
 * cleaned up.
 *
 * If the comments wrap lines, widen your emacs window.
 *
 * Peter Jensen
 * January 17, 2013
 */

#include <iostream>
#include <fstream>
#include <set>
#include <iterator>
#include "utah_set.h"
#include "node.h"

using namespace std;

int main ()
{

  void load_words_into_sets(cs3505::utah_set &our_set_of_words, cs3505::utah_set &test1_set, set<string> &stl_set_of_words, set<string> &test1_ref_set);// forward declaration 

  int pass_count = 0; // keeps track of how many tests passed
  int test_count = 0; // total number of tests   

  // Open up another block.  This way, when the block ends,
  // variables local to the block will be destroyed, but main
  // will still be running.  (Did you know that you can open
  // up a block at any time to control local variable scope and
  // lifetime?)

  {
    // Create the two sets.  Declaring the local variables constructs the objects.
    set<string>      stl_set_of_words;  // The built-in set class - no constructor parameters.
    cs3505::utah_set our_set_of_words(1000);  // Our set class, with a hashtable of 1000 slots.

    cs3505::utah_set test1_set(10);  // set for testing equal operator.
    set<string>      test1_ref_set;  // The built-in set class - no constructor parameters.

    load_words_into_sets(our_set_of_words,test1_set,stl_set_of_words,test1_ref_set);

    cout << endl;
    cout << "STL set contains " << stl_set_of_words.size() << " unique words.\n";
    cout << "Our set contains " << our_set_of_words.size() << " unique words.\n";
    cout << endl;

    // ************ BEGIN TESTING **************
    
    // Test 1 remove - count 
    const int inital_set_count = our_set_of_words.size();
    our_set_of_words.remove("newsletter");
    if (inital_set_count - 1  == our_set_of_words.size())
      pass_count++;
    else
      cout << "Test 1 remove - Failed\n Set count was not decreased when item was removed.\n";
    test_count++;

    // Test 2 remove - first of link list 
    our_set_of_words.remove("around");
    if (our_set_of_words.contains("around") == false) 
      pass_count++;
    else 
      cout << "Test 2 remove - Failed\n Set still contains a removed item\n";
    test_count++;

    // Test 3 equal - tests if the set cout is equal after '=' operation 
    our_set_of_words = test1_set;
    if (our_set_of_words.size() == test1_set.size()) 
      pass_count++;
    else 
      cout << "Test 3 equal - Failed\n New table does not have the same count after assignment\n";
    test_count++;

    // test 4 equal - tests if all words are contained after '=' operation
    bool contains_test4 = true; // true until proven false, false when set doesn't contain item
    for (set<string>::iterator it = test1_ref_set.begin(); it != test1_ref_set.end(); it++)
    {
      string s = *it;
      if (!our_set_of_words.contains(s)) {
        contains_test4 = false;
        break;
      }
    }

    if (contains_test4 && test1_ref_set.size() == our_set_of_words.size()) 
      pass_count++;
    else 
      cout << "Test 4 equal - Failed\n New table does not contain the same words after assignment\n";
    test_count++;

    // Test 5 equal empty set- tests if the set cout is equal after '=' operation 
    cs3505::utah_set empty_set(10);
    our_set_of_words = empty_set;
    if (our_set_of_words.size() == empty_set.size()) 
      pass_count++;
    else 
      cout << "Test 5- equal-empty set- Failed\n New table does not have the same count after assignment\n";
    test_count++;

    // test 6 equal empty set - tests if all words are contained after '=' operation
    bool contains_test6 = true; // true until proven false, false when set doesn't contain item
    set<string> empty_ref_set;
    for (set<string>::iterator it = empty_ref_set.begin(); it != empty_ref_set.end(); it++)
    {
      string s = *it;
      if (!our_set_of_words.contains(s)) {
        contains_test6 = false;
        break;
      }
    }

    if (contains_test6 && empty_set.size() == our_set_of_words.size()) 
      pass_count++;
    else 
      cout << "Test 6 equal empty set- Failed\n New table does not contain the same words after assignment\n";
    test_count++;

    // Reload the sets
    load_words_into_sets(our_set_of_words,test1_set,stl_set_of_words,test1_ref_set);
    {
      // test 7 get_elements - tests n items are copied into array e
      string *e = new string[test1_set.size()];

      test1_set.get_elements(e, test1_set.size());
      int elements = 0;
      for (int i = 0; i < test1_set.size() ; i++) { 
        // cout << *(e++) << endl;
        //cout << e[i] << endl;
        if (e[i] != "")
          elements++;
      }

      if (test1_set.size() == elements) 
        pass_count++;
      else 
        cout << "Test 7 - get_elements - fail\n Array count " << test1_set.size() << "  did not match original "  << elements << "  \n";
      test_count++;

      // Use the strings in e.

      delete[] e;
    }      

    {
      // test 8 get_elements - tests n that is smaller than array e and utah_set
      string *e = new string[test1_set.size()];
      int n = 10;
      test1_set.get_elements(e, n);
      
      int elements = 0;
      for (int i = 0; i < test1_set.size() ; i++) { 
        // cout << *(e++) << endl;
        //cout << e[i] << endl;
        if (e[i] != "")
          elements++;
      }

      if (n == elements) 
        pass_count++;
      else 
        cout << "Test 8 - get_elements - fail\n Array count " << test1_set.size() << "  did not match original "  << elements << "  \n";
      test_count++;

      // Use the strings in e.

      delete[] e;
    }      

    {
      // test 9 get_elements - tests n that is larger than utah_set
      string *e = new string[30];
      int n = 30;
      test1_set.get_elements(e, n);
      
      int elements = 0;
      for (int i = 0; i < test1_set.size() ; i++) { 
        // cout << *(e++) << endl;
        //cout << e[i] << endl;
        if (e[i] != "")
          elements++;
      }

      if (20 == elements) 
        pass_count++;
      else 
        cout << "Test 9 - get_elements - fail\n Array count " << test1_set.size() << "  did not match original "  << elements << "  \n";
      test_count++;

      // Use the strings in e.

      delete[] e;
    }      

    {
      // test 10 get_elements - tests n elements were inserted into e, large case
      string *e = new string[our_set_of_words.size()];
      int n = our_set_of_words.size();
      our_set_of_words.get_elements(e, n);
      
      int elements = 0;
      for (int i = 0; i < our_set_of_words.size() ; i++) { 
        // cout << *(e++) << endl;
        //cout << e[i] << endl;
        if (e[i] != "")
          elements++;
      }

      if (n == elements) 
        pass_count++;
      else 
        cout << "Test 10 - get_elements - fail\n Array count " << our_set_of_words.size() << "  did not match original "  << elements << "  \n";
      test_count++;

      // Use the strings in e.

      delete[] e;
    }      
    
    {
      // test 11 get_elements - tests if elements are the same
      string *e = new string[our_set_of_words.size()];
      int n = our_set_of_words.size();
      our_set_of_words.get_elements(e, n);
      
      int elements = 0;
      set<string> elements_from_e;
      for (int i = 0; i < our_set_of_words.size() ; i++) { 
        elements_from_e.insert(e[i]);
      }

      // compare the each string
      bool compare_passed = true; // true until proven false
      
      set<string>::iterator it2 = elements_from_e.begin();
      for (set<string>::iterator it = stl_set_of_words.begin(); it != stl_set_of_words.end() && it2 != elements_from_e.end(); it++) {
        string control = *it;
        string testing = *it2;
        if (control != testing) {
          // the words didn't match
         
          compare_passed = false;
          break;
        }
        it2++;
      }
      
      if (compare_passed) 
        pass_count++;
      else 
        cout << "Test 11 get_elements - failed\n Elements in array e differed from passed in array\n";
      test_count++;

      // Use the strings in e.

      delete[] e;
    }      
    // Done.  Notice that this code block ends below.  Any local
    // variables declared within this block will be automatically
    // destroyed.  Local objects will have their destructors
    // called.
  }

  // Now that the objects have been destroyed, I will simply call my auditing
  // code to print out how many times constructors have been called, and
  // how many times destructors have been called.  They should exactly match.
  // If not, we have a memory problem.

  cout << "Class cs3505::utah_set:" << endl;
  cout << "    Objects created:  " << cs3505::utah_set::constructor_count() << endl;
  cout << "    Objects deleted:  " << cs3505::utah_set::destructor_count() << endl;
  cout << endl;

  cout << "Class cs3505::node:" << endl;
  cout << "    Objects created:  " << cs3505::node::constructor_count() << endl;
  cout << "    Objects deleted:  " << cs3505::node::destructor_count() << endl;
  cout << endl;

  // Test end memory - tests if constructor count equals destructor cout 
  if (cs3505::node::constructor_count() == cs3505::node::destructor_count()) 
    pass_count++;
  else 
    cout << "End memory test failed. Constructor count doesn't equal destructor count\n";
  test_count++;

  // Print test pass or fail
  pass_count == test_count ? cout << "Tests passed\n" : cout << "Tests failed\n";
  cout << endl;

  return 0;
}

void load_words_into_sets(cs3505::utah_set &our_set_of_words, cs3505::utah_set &test1_set, set<string> &stl_set_of_words, set<string> &test1_ref_set) 
{

  // Open the file stream for reading.  (We'll be able to use it just like
  //   the keyboard stream 'cin'.)

  ifstream in("Yankee.txt");

  // Loop for reading the file.  Note that it is controlled
  //   from within the loop (see the 'break').

  while (true)
  {
    // Read a word (don't worry about punctuation)

    string word;
    in >> word;

    // If the read failed, we're probably at end of file
    //   (or else the disk went bad).  Exit the loop.

    if (in.fail())
      break;

    // Word successfully read.  Add it to both sets.

    stl_set_of_words.insert(word);
    our_set_of_words.add(word);
  }

  // Close the file.

  in.close();

  ifstream test1("test1.txt");
  // Loop for reading the file.  Note that it is controlled
  //   from within the loop (see the 'break').

  while (true)
  {
    // Read a word (don't worry about punctuation)

    string test1_word;
    test1 >> test1_word;

    // If the read failed, we're probably at end of file
    //   (or else the disk went bad).  Exit the loop.

    if (test1.fail())
      break;

    // Word successfully read.  Add it to both sets.
    test1_set.add(test1_word);
    test1_ref_set.insert(test1_word);
  }

  // Close the file.

  test1.close();
}

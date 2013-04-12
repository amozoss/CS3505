/* A 'utah set' is defined as a set of strings stored
 * in a hashtable that uses chaining to resolve collisions.
 *
 * Peter Jensen
 * January 17, 2013
 */

#ifndef UTAH_SET_H
#define UTAH_SET_H

#include<string>

namespace cs3505
{
  class node;  // A forward declaration of the class.  We can now
               // create pointer variables involving the node class.
  
  class utah_set
  {
  public:
    utah_set(int capacity = 10);      // Constructor
    utah_set(const utah_set & other); // Copy constructor
    ~utah_set();                      // Destructor

    void add      (std::string element);  // Not const - modifies the object
    void remove   (std::string element);  // Not const - modifies the object
    bool contains (std::string element) const;  // const - does not change the object
    int  size     () const;               // Const - does not change object
    
    void get_elements(std::string *array, int n) const; //  This function populates an array of strings with elements from the set.

    const utah_set & operator= (const utah_set & rhs);  // Modifies this object

     
  private:
    int hash (std::string s) const;  // Const - does not change this object
    void clean ();                   // Not const - modifies the object
    
    int capacity;  // The size of the hashtable
    int count;     // The number of elements in the set
    node** table;  // The hashtable, a pointer to a node pointer
                   //   (which will really be an array of node pointers

    // Below are a few definitions that I use to make sure I am cleaning
    //   up memory properly.  (Notice that I can switch back to public / private
    //   as many times as I like.)  These are not part of a normal
    //   hashtable, but they are here because they help with debugging.
    // These functions and variables are declared static - they exist in the
    //   class, but not in any particular object.  (The objects can access
    //   them, of course.)
    
  public:
    static long long constructor_count ();  // A debugging function for Peter
    static long long destructor_count ();   // A debugging function for Peter

  private:
    static long long constructor_calls;  // A debugging variable for Peter
    static long long destructor_calls;   // A debugging variable for Peter
    
  };

 
}

#endif

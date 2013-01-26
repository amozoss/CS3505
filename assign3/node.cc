/* This node class is used to build linked lists for the
 * utah_set class.
 *
 * Peter Jensen
 * January 17, 2013
 */

#include <string>
#include "node.h"

namespace cs3505
{
  /*******************************************************
   * node member function definitions
   ***************************************************** */

  /** Constructor:  Creates a node containing
    *   an element.  It is initialized to
    *   not point to any other node.
    */
  node::node(const std::string & e)
    : element(e)  // This syntax is used to call member variable constructors
  {
    // Do not change or remove the following line.
    //   It must execute for our auditing to be correct.

    node::constructor_calls++;

    // Normal constructor tasks below.

    this->next = NULL;  // This node points to no other node.
  }

  
  /** Destructor:  release any memory allocated
    *   for this object.
    */
  node::~node()
  {
    // Do not change or remove the following line.
    //   It must execute for our auditing to be correct.

    node::destructor_calls++;

    // Normal destructor tasks below.
    
    // (Nothing to do.)
  }

  
  /*******************************************************
   * node static definitions:
   *
   *     These are for debugging purposes only.  They help 
   * the programmer audit their memory usage.
   *
   *     Do not change anything below this point.
   ***************************************************** */

  // We haven't covered static class members.  Since static
  // variables are not in objects, we need to define storage
  // for them.  These variables are -here-, not in any object.
  // (This is the ONLY copy of these variables that will exist.)
  
  long long node::constructor_calls = 0;
  long long node::destructor_calls = 0;


  /** Returns the number of times any node
    *   constructor has been called.
    */
  long long node::constructor_count ()
  {
    return node::constructor_calls;
  }

  
  /** Returns the number of times the node
    *   destructor has been called.
    */
  long long node::destructor_count ()
  {
    return node::destructor_calls;
  }
}

/* Dan Willoughby
 * U0559245
 * Assignment 2
 *
 * huge_number represents non-negative integers of an arbitrary number of digits. 
 */

// Is the label defined?
#ifndef HUGE_NUMBER_H

// If not, define it and compile the rest of it.
#define HUGE_NUMBER_H

#include <ostream>
#include <string>

using namespace std;
class huge_number
{
public:
  huge_number(string s); // public constructor
  huge_number();         // default constructor
  huge_number(const huge_number& hn); // copy constructor
  string get_value (); // public accessor

  // overload of =,+,-,*,/,% operators
  huge_number & operator=(const huge_number &rhs);
  huge_number operator+(const huge_number &hn);
  huge_number operator-(const huge_number &hn);
  huge_number operator*(const huge_number &hn);
  huge_number operator/(const huge_number &hn);
  huge_number operator%(const huge_number &lhs);

private:
  string value; // value string represents an arbitrary number of digits
  // Forward declarations

  string subtract (string, string);
  string add (string a, string b);
  string multiply (string, string);
  string divide (string, string, bool); // true for quotient, false for remainder
};

// marks end of compiler flags.
#endif

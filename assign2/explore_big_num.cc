/* Test code for manipulating strings  - assumes non-negative strings */

#include <iostream>
#include <string>

using namespace std;

// Forward declarations

string add (string, string);
string multiply (string a, string b);

/*
 * Main - the application entry point
 *
 * This application was constructed just to test algorithms for
 * adding numbers represented as strings.  Note:  This is not robust.
 * Care should be taken to ensure that the strings only have numeric
 * digits within them.
 *
 */
int main ()
{
  string s = "12345";
  string t =   "987";

  cout << s << endl;
  cout << t << endl;
  cout << endl;

  string u;

  u = add(s, t);

  cout << u << endl;
  cout << endl;
  
  u = multiply(s, t);

  cout << u << endl;
  cout << endl;
  
  return 0;  // I forgot this in my other examples.
             // Returning '0' signals 'no error' to the shell.
}

/*
 * Adds two numbers stored in strings, building a string result.
 */

string add (string a, string b)
{
  // Build up a string object to contain the result.
  
  string result = "";

  // Work right to left.  (Most significant digits to least)
  
  int a_pos = a.length() - 1;
  int b_pos = b.length() - 1;
  
  int carry = 0;

  // Loop, adding columns, until no more columns and no carry.
  
  while (a_pos >= 0 || b_pos >= 0 || carry > 0)
    {
      // Get next digit from each string, or 0 if no more.
    
      int a_digit = a_pos >= 0 ? a[a_pos--]-'0' : 0;
      int b_digit = b_pos >= 0 ? b[b_pos--]-'0' : 0;

      /// Calculate the digit for this column and the carry out
    
      int sum = a_digit + b_digit + carry;
      carry = sum / 10;
      sum = sum % 10 + '0';

      // Put this column's digit at the start of the result string.
    
      result.insert(0, 1, static_cast<char>(sum));
    }

  // Strip any leading 0's  (shouldn't be any, but you'll use this elsewhere.)

  while (result[0] == '0')
    result.erase(0, 1);

  // If the string is empty, it is a 0.
  
  if (result.length() == 0)
    result = "0";

  // Return the result.
  
  return result;
}

/*
 * Multiplies two numbers stored in strings, building a string result.
 */

string multiply (string a, string b)
{
  string result = "0";

  int b_pos = b.length() - 1;

  // Loop once for each digit in b.
  while (b_pos >= 0)
    {
      int b_digit = b[b_pos--]-'0';  // Get next digit from b.

      // Add a to the result the appropriate number of times.
      
      for (int i = 0; i < b_digit; i++)
	{
	  result = add(result, a);
	  // cout << "  " << result << endl;  // Debug only
	}

      // Multiply a by 10.
      
      a.append("0");

      // Debug only.  (Useful to have string numbers for this!)      
      // cout << a << endl;
    }

  return result;
}

// Other really useful functions

//is_bigger (a, b)
//subtract (a, b)
//divide (a, b)
//mod (a, b)


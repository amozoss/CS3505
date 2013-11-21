/* Dan Willoughby
 * U0559245
 * Assignment 2
 *
 * huge_number class definitions  
 */

#include "huge_number.h"
#include <iostream>
#include <sstream>

using namespace std;

/*
 * Public constructor definition
 */
huge_number::huge_number (string s)
{
  this->value = s;
}

/*
 * Default Constructor definition
 */
huge_number::huge_number ()
{
  this->value = "0";
}

/*
 * Copy constructor definition
 */
huge_number::huge_number (const huge_number& hn)
{
  this->value = hn.value;
}

/* Public accessor 
 * 
 * Returns the string representation of the number
 */
string huge_number::get_value ()
{
  string s = this->value;
  return s;
}

/*
 * Overload of = opeartor
 */
huge_number & huge_number::operator= (const huge_number &rhs)
{
  this->value = rhs.value;

  return *this;
}

/*
 * Overload of + opeartor
 */
huge_number huge_number::operator+ (const huge_number &hn)
{
  return huge_number(huge_number::add(this->value,hn.value));
}

/*
 * Overload of - opeartor
 */
huge_number huge_number::operator- (const huge_number &hn)
{
  huge_number e;
  e = huge_number(huge_number::subtract(this->value,hn.value));

  // check for negative numbers, subtract returns -1 if negative so change it to zero.
  if (e.get_value().compare("-1") == 0)
    e = huge_number("0"); 
  return e;
}

/*
 * Overload of * opeartor
 */
huge_number huge_number::operator* (const huge_number &hn)
{
  return huge_number(huge_number::multiply(this->value, hn.value));
}

/*
 * Overload of / opeartor
 */
huge_number huge_number::operator/ (const huge_number &hn)
{
  return huge_number(huge_number::divide(this->value,hn.value,true));
}

/*
 * Overload of % opeartor
 */
huge_number huge_number::operator% (const huge_number &lhs)
{
  return huge_number(huge_number::divide(this->value,lhs.value,false));
}

/*
 * Subtracts two numbers stored in strings, building a string result. If result is less
 * than zero, returns -1  ( no negative numbers)
 */
string huge_number::subtract (string a, string b)
{

  // Build up a string object to contain the result.
  
  string result = "";

  // Work right to left.  (Most significant digits to least)
  
  int a_pos = a.length() - 1;
  int b_pos = b.length() - 1;
  
  //int borrow = 0;

  // Loop, adding columns, until no more columns and no borrow.
  
  while (a_pos >= 0 || b_pos >= 0)
  {
      // Get next digit from each string, or 0 if no more.
    
      int a_digit = a_pos >= 0 ? a[a_pos--]-'0' : 0;
      int b_digit = b_pos >= 0 ? b[b_pos--]-'0' : 0;
      
      //cout << "a: " << a_digit << endl;
      //cout << "b: " << b_digit << endl;

      int diff = a_digit - b_digit;
      //cout << "diff: " << diff << endl;
     
      // Perform borrow operation if needed
      if (diff < 0) { 
        //cout << "borrow\n";
        int borrow_pos = a_pos; 
      
        bool borrow_succeeded = false;
         // checks each number position to see if borrowing is possible
        while (borrow_pos >= 0) {
          int borrow_digit = a[borrow_pos]-'0';
          
          if (borrow_digit > 0) { // borrowing is possible
            a[borrow_pos]--; // borrow
            
            // Deals with zero special case replaces zero with 9 
            while (borrow_pos != a_pos) {
              if (a[++borrow_pos]-'0' == 0)
                a[borrow_pos] = 9 + '0'; 
            } 

            a_digit += 10; // borrowing
            diff = a_digit - b_digit;
            borrow_succeeded = true;
            break;
          }
          else { // check the next place
             borrow_pos--;
          }
        }
        if (!borrow_succeeded)
          // it was a negative number
          return "-1";
      }
      //cout << "diff: " << diff << endl;
      diff = diff % 10 + '0';

      // Put this column's digit at the start of the result string.
      result.insert(0, 1, static_cast<char>(diff));
  }
  // Strip any leading 0's  (shouldn't be any, but you'll use this elsewhere.)

  while (result[0] == '0') {
    //cout <<"zero: " << result[0] << endl;
    result.erase(0, 1);
  }

  // If the string is empty, it is a 0.
  
  if (result.length() == 0)
    result = "0";

  // Return the result.
  
  return result;
}

/*
 * Adds two numbers stored in strings, building a string result.
 */
string huge_number::add (string a, string b)
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

string huge_number::multiply (string a, string b)
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
bool isAllZeros(string a)
{
  bool isZero = true;
  for (int i = 0; i < a.length(); i++)
    if (a.at(i) - '0' != 0) {
      isZero = false;
      break;
    }
  return isZero;
}

/*
 * Divides two numbers stored in strings, building a string result.
 * Computes string a / string b. When isDivide is true returns quotient, 
 * otherwise returns remainder (mod)
 */
string huge_number::divide (string a, string b, bool isDivide)
{
  // divid by zero
  if (isAllZeros(b))
    throw exception();
  // if b is larger the result is 0
  if (a.length() < b.length())
    return "0";
  
  int a_pos = 1;

  string calc = a.substr(0,1);

  string remainder = "";
  string quotient = "";

  // while loop is terminated with a break statement
  while (true)
  {
    remainder = calc;
    int result = 0;// counts how many times it was divided

    // repeatedly subtract until the number becomes negative
    while (calc.compare("-1") != 0)
    {
      calc = subtract(calc, b);
      if (calc.compare("-1") != 0 ) // determines remainder
        remainder = calc;
      if ( calc.compare("-1") != 0)
        result++; // counts how many times it was divided
    }

    // append result to quotient
    quotient += static_cast<char>(result % 10 + '0');

    if (a_pos > a.length()-1) // reached the last digit in a
      break;

    remainder += a.at(a_pos++); // append the next digit
    calc = remainder;
  }

  // Strip any leading 0's  (shouldn't be any, but you'll use this elsewhere.)
  //cout << "Remainder: " << remainder << endl;
  while (quotient[0] == '0')
    quotient.erase(0, 1);

  // If the string is empty, it is a 0.
  if (quotient.length() == 0)
    quotient = "0";

  // Strip any leading 0's  (shouldn't be any, but you'll use this elsewhere.)
  while (remainder[0] == '0')
    remainder.erase(0, 1);

  // If the string is empty, it is a 0.
  if (remainder.length() == 0)
    remainder = "0";

  // Return the quotient.
  return isDivide ? quotient : remainder;
}

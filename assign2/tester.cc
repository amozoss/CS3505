#include <iostream>
#include "huge_number.h"

using namespace std;

int main ()
{
  huge_number b = huge_number("9852");
  huge_number c = huge_number("1000000000000000000000000000000000009852");
  huge_number a;
  huge_number d;
  huge_number e;

  // test add
  a = a + b;
  //cout << a.get_value () << endl;

  // test mult
  d = c * b;
  //cout << d.get_value () << endl;

  // test subtract
  // test 1
  cout << "Subtract test 1" << endl;
  e = c - b;
  e.get_value ().compare("1000000000000000000000000000000000000000") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // test 2 negative number
  cout << "Subtract test 2" << endl;
  b = huge_number("4564");
  c = huge_number("4563");
  e = c - b;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // test 3 zero number

  cout << "Subtract test 3" << endl;
  b = huge_number("12");
  c = huge_number("3");
  e = b - c;
  e.get_value ().compare("9") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // test 4 pos number
  cout << "Subtract test 4" << endl;
  b = huge_number("4564");
  c = huge_number("4565");
  e = c - b;
  e.get_value ().compare("1") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // test 5 
  cout << "Subtract test 5" << endl;
  b = huge_number("100000000000000000");
  c = huge_number("1");
  e = b - c;
  e.get_value ().compare("99999999999999999") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // test 6 
  cout << "Subtract test 6" << endl;
  b = huge_number("100000000000000000000000000000000000000000000000000000000000");
  c = huge_number("100000000000000000000000000000000000000000000000000000000000");
  e = b - c;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // test 7 
  cout << "Subtract test 7" << endl;
  b = huge_number("100000000000000000");
  c = huge_number("1");
  e = c - b;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;


  // divide test 1
 cout << "**********************Divide test 1**********************" << endl;
  b = huge_number("14");
  c = huge_number("3");
  e = b / c;
  e.get_value ().compare("4") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;

  // divide test 2 big numbers
 cout << "**********************Divide test 2**********************" << endl;
  b = huge_number("9999999999999999999999999999999999999999");
  c = huge_number("3");
  e = b / c;
  e.get_value ().compare("3333333333333333333333333333333333333333") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  
 //  divide test 3 
 cout << "**********************Divide test 3**********************" << endl;
  b = huge_number("102");
  c = huge_number("3");
  e = b / c;
  e.get_value ().compare("34") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  
  // divide test 4 big numbers
 cout << "**********************Divide test 4**********************" << endl;
  b = huge_number("909876543212345678909876543214");
  c = huge_number("309876543212345678909876543214");
  e = b / c;
  e.get_value ().compare("2") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // divide test 5 big numbers
 cout << "*****************Divide test 5**********************" << endl;
  b = huge_number("909876543212345678909876543214");
  c = huge_number("32345678909876543214");
  e = b / c;
  e.get_value ().compare("28129771081") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // divide test 6 big numbers
 cout << "**********************Divide test 6**********************" << endl;
  b = huge_number("1000000000000000000000000000000000000000");
  c = huge_number("50");
  e = b / c;
  e.get_value ().compare("20000000000000000000000000000000000000") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // divide test 7 big numbers
 cout << "*****************Divide test 7**********************" << endl;
  b = huge_number("909876543212345678909876543214");
  c = huge_number("32");
  e = b / c;
  e.get_value ().compare("28433641975385802465933641975") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // divide test 8 big numbers
 cout << "*****************Divide test 8**********************" << endl;
  b = huge_number("00000000000000000");
  c = huge_number("3");
  e = b / c;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
 
  // mod test 1
 cout << "**********************Mod test 1**********************" << endl;
  b = huge_number("14");
  c = huge_number("3");
  e = b % c;
  e.get_value ().compare("2") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;

  // mod test 2 big numbers
 cout << "**********************Mod test 2**********************" << endl;
  b = huge_number("9999999999999999999999999999999999999999");
  c = huge_number("3");
  e = b % c;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  
 //  mod test 3 
 cout << "**********************Mod test 3**********************" << endl;
  b = huge_number("102");
  c = huge_number("3");
  e = b % c;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  
  // mod test 4 big numbers
 cout << "**********************Mod test 4**********************" << endl;
  b = huge_number("909876543212345678909876543214");
  c = huge_number("309876543212345678909876543214");
  e = b % c;
  e.get_value ().compare("290123456787654321090123456786") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // mod test 5 big numbers
 cout << "*****************Mod test 5**********************" << endl;
  b = huge_number("909876543212345678909876543214");
  c = huge_number("32345678909876543214");
  e = b % c;
  e.get_value ().compare("17988888328452548880") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // mod test 6 big numbers
 cout << "**********************Mod test 6**********************" << endl;
  b = huge_number("1000000000000000000000000000000000000000");
  c = huge_number("50");
  e = b % c;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // mod test 7 big numbers
 cout << "*****************Mod test 7**********************" << endl;
  b = huge_number("909876543212345678909876543214");
  c = huge_number("32");
  e = b % c;
  e.get_value ().compare("14") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;
  // mod test 8 big numbers
 cout << "*****************Mod test 8**********************" << endl;
  b = huge_number("0");
  c = huge_number("34");
  e = b % c;
  e.get_value ().compare("0") == 0 ? cout << "PASSED\n" : cout << "FAILED\n";
  cout << e.get_value () << endl;



  // additional tests
  string r = "123";
  string t = "456";
  huge_number x(r);
  huge_number y(t);
  huge_number z;

  z = x * y - x;

  cout << x.get_value() << endl;  // X should not have changed
  cout << y.get_value() << endl;  // Y should not have changed
  cout << z.get_value() << endl;
}


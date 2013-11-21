
#include <iostream>  // needed for cout and cin
#include <cstdlib>
#include <cmath>
#include <iomanip>

using namespace std;

int main ()
{
  cout << "Enter a weight in pounds: ";

  double weight;

  cin >> weight; // keyboard input

  double kilos  = weight * 0.453592; // convert to kilograms

  cout <<  setprecision (2) << fixed << showpoint <<  weight << " pounds is " <<  kilos << " kilograms." << endl;

  
  
}

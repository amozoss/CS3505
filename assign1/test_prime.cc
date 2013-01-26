#include <iostream>  // needed for cout and cin
#include <cstdlib>
#include <cmath>
#include <cctype>

using namespace std;

int main (int argc, char *argv[])
{
  // Check console parameter count
  if (argc != 2) {
    cout << "Invalid number of parameters.\n";
    return 0;
  } 

  // parse
  int prime = atoi(argv[1]);
  
  if (prime <= 1) { // check for negative numbers, zero, and one
    cout << "Invalid input. Must be a non-zero positive integer greater than 1.\n";
    return 0;
  }

  for (int i = 2; i < prime; i++) {
    if (prime % i == 0) {
      cout << "composite\n";
      return 0;
    }
  }
  cout << "prime\n";
  
  
}

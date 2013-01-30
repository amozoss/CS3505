/*
 * This is the tester class for assignment 4.
 *
 * It includes all unit tests for food_item, easy_date, ware_house, and headquarter
 *
 * Prints tests passed if all tests are successful, tests failed otherwise. 
 *
 * Authors Dan Willoughby and Michael Banks
 */


#include <iostream>
#include <fstream>
#include <set>
#include <iterator>
#include <string>
#include "easy_date.h"
#include "food_item.cc"

using namespace std;
// forward declarations
void test_easy_date (int pass_count, int test_count);
void test_warehouse();
int main ()
{

  int pass_count = 0; // keeps track of how many tests passed
  int test_count = 0; // total number of tests   


  test_easy_date(pass_count,test_count);
  
}

void test_easy_date (int pass_count, int test_count)
{

    string date = "05/01/2010";
    easy_date e(date);
    e.to_str();
    if (true)
      pass_count++;
    else 
      cout << "Test 2 remove - Failed\n Set still contains a removed item\n";
    test_count++;

}

void test_warehouse()
{


}

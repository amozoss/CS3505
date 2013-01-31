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
#include "food_item.h"
#include "transaction.h"

using namespace std;
// forward declarations
void test_easy_date (int pass_count, int test_count);
void test_warehouse();
void test_transaction();
void test_food_item();


int main ()
{

  int pass_count = 0; // keeps track of how many tests passed
  int test_count = 0; // total number of tests   

  test_warehouse();
  test_transaction();
  test_food_item();
  // test_easy_date(pass_count,test_count);
  
}

void test_easy_date (int pass_count, int test_count)
{

//    string date = "05/01/2010";
 //   easy_date e(date);
 //   e.to_str();
    if (true)
      pass_count++;
    else 
      cout << "Test 2 remove - Failed\n Set still contains a removed item\n";
    test_count++;

}

void test_warehouse()
{


}

void test_transaction()
{
  transaction h("Request: 0984523912 7 Tacoma", "hello");
}

void test_food_item()
{
  food_item f("FoodItem - UPC Code: 0556467522  Shelf life: 15  Name: absolute vanilla vodka");
  cout << f.get_UPC() << " is UPC\n" << f.get_name() << " is name\n" << f.get_shelf_life() << " is shelf life\n";
}

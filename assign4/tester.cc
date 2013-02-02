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
#include <map>
#include <iterator>
#include <string>
#include "headquarters.h"
#include "easy_date.h"
#include "food_item.h"
#include "transaction.h"
#include "warehouse.h"


using namespace std;
// forward declarations
void test_easy_date (int pass_count, int test_count);
void test_warehouse();
void test_transaction();
void test_food_item();
void data1_test ();
headquarters headquarters_test("data1.txt");


int main ()
{

  int pass_count = 0; // keeps track of how many tests passed
  int test_count = 0; // total number of tests   

 // test_warehouse();
 // test_transaction();
 // test_food_item();
 // test_easy_date(pass_count,test_count);
  data1_test();
  
}

void data1_test ()
{
  {
  cout << "entering test" << endl;
  cout << headquarters_test.get_warehouse("Columbus").get_name() << endl;;
  set<string> columbus =headquarters_test.get_warehouse("Columbus").report_foods_in_stock();
  //w.report_foods_in_stock();
  for (set<string>::iterator it = columbus.begin(); it != columbus.end(); it++)
    cout << *it << endl;
  cout << "end test" << endl;
  }

  {
  warehouse w = headquarters_test.get_warehouse("Columbus");
  cout << "test_warehouse: " << w.get_name();
  w.report_foods_in_stock();
 // for (set<string>::iterator it = columbus.begin(); it != columbus.end(); it++)
   // cout << *it << endl;
  }
}

void test_easy_date(int pass_count, int test_count)
{

  ifstream in("dates.txt");
  easy_date begin_date("01/01/2001");

  while (true)
  {
    // Read a line 
    string line;
    getline(in,line);

    // If the read failed, we're probably at end of file
    //   (or else the disk went bad).  Exit the loop.
    if (in.fail()) {
      break;
    }


    //test easy dates
    easy_date date(line);
    //cout << line << endl;
    if (date.to_str() != line)
      cout << "easy_date constuctor was " << date.to_str() << " should be " << line << endl;
    
    string next_date = begin_date.to_str();
    //cout << "begin_date " << begin_date.to_str() << endl;

    if (next_date != line)
      cout << "easy_date next_day() was " << next_date << " should be " << line << endl;
    //cout << "begin_date " << begin_date.to_str() << endl;

    begin_date.next_date();


  }
  in.close();


}

void test_warehouse()
{
  map<string, food_item> da_set;
  warehouse w("Tacoma", da_set);

  w.add_transaction("Request: 0984523912  7 Tacoma");

}

void test_transaction()
{
  transaction h("Request: 0984523912 7 Tacoma", "hello");
  h.set_shelf_life(7);
}

void test_food_item()
{
  food_item f("FoodItem - UPC Code: 0556467522  Shelf life: 15  Name: absolute vanilla vodka");
  cout << f.get_UPC() << " is UPC\n" << f.get_name() << " is name\n" << f.get_shelf_life() << " is shelf life\n";
}

/* 
 * warehouse.h
 * 
 * A warehouse keeps manages transactions, shelf lives of food, and reports foods in stock
 * or foods in deficit
 *
 * Dan Willoughby and Michael Banks
 */

#ifndef WAREHOUSE_HEADER_IKL
#define WAREHOUSE_HEADER_IKL

#include "food_item.h"
#include <string>
#include "easy_date.h"
#include <list>
#include <set>
#include <map>
#include "transaction.h"
#include <iostream>
#include <sstream>
#include <cstdlib>
#include <sstream>
#include <algorithm>
#include <iterator>
#include <vector>
#include <queue>

using namespace std;
//
class warehouse{
 public:
  warehouse(string warehouse_data, map<string, food_item> );
  warehouse(string warehouse_data, map<string, food_item> * ptr);
  warehouse();
  warehouse(const warehouse & other); // Copy constructor
  ~warehouse();

  void add_transaction(string trans); 
  //void add_transaction(string, map<string, food_item>);
  string report_busiest_day();        // Returns name, date and transaction quantity of busiest day.
  set<string> report_food_deficit(); // UPC numbers of foods out of stock
  set<string> report_foods_in_stock(); // UPC numbers of the foods in stock
  void forward_date();           // Forward date will also update shelf life of food items.
  string get_name();             // Returns the name of the warehouse.
  void set_start_date(string);   // Sets the start date of transactions.
  map<string, int> food_inventory; // value = UPC, int = current quantity

 private:
  string name;
  easy_date effective_date;   // If a request/receive is given, this is the date is happened on.
  map<string, food_item>* food_ptr;
  //map<string, food_item> foods; // copies in the whole food map, simply avoid the hassle of pointers
  string convert_int_to_str(int);
  string convert_char_to_str(char);
    list<transaction> trans_list;  // trans_list is a list of all transactions of this warehouse.
    //  queue<transaction> trans_q;    // trans_q is a queue of all transactions of the warehouse. Used to keep the order of transactions.
    list<string> dates; // A list of all the dates this warehouse operated. 
};
#endif

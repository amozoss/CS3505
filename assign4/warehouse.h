/* 
 * warehouse.h
 * Warehouse file comment
 *
 * Dan Willoughby and Michael Banks
 */

#ifndef WAREHOUSE_HEADER_IKL
#define WAREHOUSE_HEADER_IKL

#include "food_item.h"
#include "easy_date.h"
#include <list>
#include <set>
#include <map>
#include "transaction.h"
using namespace std;

class warehouse{
 public:
  warehouse(string warehouse_data, map<string, food_item> );
  ~warehouse();

  void add_transaction(string trans); 
  string report_busiest_day();        // Returns name, date and transaction quantity of busiest day.
  set<string> report_food_deficit(); // UPC numbers of foods out of stock
  set<string> report_foods_in_stock(); // UPC numbers of the foods in stock
  void forward_date();        // Forward date will also update shelf life of food items.
  string get_name();
  void set_start_date(string);

  private:
  string name;
  easy_date effective_date;   // If a request/receive is given, this is the date is happened on.
  map<string, food_item> foods; // copies in the whole food map, simply avoid the hassle of pointers
  string convertInt_toString();
  list<transaction> trans_list;  // trans_list is a list of all transactions of this warehouse.
  list<transaction>::iterator iter;

};
#endif

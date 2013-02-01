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
#include "transaction.h"
using namespace std;

class warehouse{
 public:
  warehouse(string name, set<food_item> *foodSet);
  ~warehouse();

  void add_transaction(string trans); 
  string reportBusiestDay();        // Returns name, date and transaction quantity of busiest day.
  string reportFoodDeficit();
  list<food_item> reportFoodsInStock();
  void forwardDate();        // Forward date will also update shelf life of food items.

 private:
  string name;
  easy_date effectiveDate;   // If a request/receive is given, this is the date is happened on.
  set<food_item> *foods;
  string convertInt_toString();
  list<transaction> *trans_list;  // trans_list is a list of all transactions of this warehouse.

};
#endif

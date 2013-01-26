/* 
 * warehouse.h
 * Warehouse file comment
 *asdf
 *
 * Dan Willoughby and Michael Banks
 */

#ifndef WAREHOUSE_HEADER
#define WAREHOUSE_HEADER

#include <string>
#include <set>
#include <list>
#include "food_item.cc"
#include "date.cc"

using namespace std;

class warehouse{
 public:
  warehouse(string name, set<food_item> &foodSet);
  ~warehouse();

  void receive(string theReceive);
  void request(string theRequest);
  string reportBusiestDay();        // Returns name, date and transaction quantity of busiest day.
  string reportFoodDeficit();
  list<food_item> reportFoodsInStock();
  void forwardDate();

 private:
  string name;
  date effectiveDate;
  set<food_item> *foods;
  string convertInt_toString();


};
#endif

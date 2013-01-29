/* 
 * warehouse.h
 * Warehouse file comment
 *asdf
 *
 * Dan Willoughby and Michael Banks
 */

#ifndef WAREHOUSE_HEADER_IKL
#define WAREHOUSE_HEADER_IKL

#include "headquarters.h"

using namespace std;

class warehouse{
 public:
  warehouse(string name, set<food_item> *foodSet);
  ~warehouse();

  void receive(string theReceive);
  void request(string theRequest);  
  string reportBusiestDay();        // Returns name, date and transaction quantity of busiest day.
  string reportFoodDeficit();
  list<food_item> reportFoodsInStock();
  void forwardDate();

 private:
  string name;
  easy_date effectiveDate;
  set<food_item> *foods;
  string convertInt_toString();


};
#endif

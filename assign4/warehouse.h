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

using namespace std;

class warehouse{
 public:
  warehouse(string name);
  ~warehouse();
  /**/
  void receive();
  void request();
  string reportBusiestDay();
  string effectiveDate();
  string reportFoodDeficit();
  string reportFoodSurplus();
  void forwardDate();

 private:
  string name;


  string convertInt_toString();


};

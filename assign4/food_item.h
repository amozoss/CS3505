/*
 *food_item.h
 */
#ifndef FOOD_ITEM
#define FOOD_ITEM

#include <string>

using namespace std;

class food_item {
public:
  string get_UPC();
  string get_name();
 void dec_shelf_life();
  int get_shelf_life();
private:
  string upc_code;
  int shelf_life;
  string name;
 

  food_item(const string upc_code,int shelf_life, const string name); // public constructor
};


#endif

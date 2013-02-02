/*
 *food_item.h
 *
 * Food items have a UPC, name, and shelf life
 */
#ifndef FOOD_ITEM
#define FOOD_ITEM

#include <string>

using namespace std;

class food_item {
public:
  food_item(string);
  food_item();
  ~food_item();
  string get_UPC();
  string get_name();
  int get_shelf_life();
private:
  string upc_code;
  int shelf_life;
  string name;
  food_item(const string upc_code,int shelf_life, const string name); // public constructor
};
#endif

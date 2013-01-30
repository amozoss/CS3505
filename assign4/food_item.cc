/* food_item.cc
 *
 * A food item has: 
 *  UPC Code (single string of numbers)
 *  Shelf life (number in days)
 *  name (a string of words) 
 */
#ifndef FOOD_ITEM
#define FOOD_ITEM

#include <string>

using namespace std;

class food_item {
public:
  string getUPC();
  string getName();
 void decShelfLife();
private:
  string upc_code;
  int shelf_life;
  string name;
 

  food_item(const string upc_code,int shelf_life, const string name); // public constructor
};



// implementation
food_item::food_item(const string upc_code,int  shelf_life, const string name) {
  this->upc_code = upc_code;
  this->shelf_life = shelf_life;
  this->name = name;

}

void food_item::decShelfLife()
{
  if(shelfLife > 0)
    shelfLife--;
}

#endif



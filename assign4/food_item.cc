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
  int getShelfLife();
private:
  string upc_code;
  int shelf_life;
  string name;
 

  food_item(const string upc_code,int shelf_life, const string name); // public constructor
};


#endif

// implementation
food_item::food_item(const string upc_code,int  shelf_life, const string name) {
  this->upc_code = upc_code;
  this->shelf_life = shelf_life;
  this->name = name;

}

/*
 * Subtracts one from the shelf life of the food. Stops when shelf life equals 0.
 */
void food_item::decShelfLife()
{
  if(shelf_life > 0)
    shelf_life--;
}
/*
 * Returns the upc code of the food.
 */
string food_item::getUPC()
{
  return upc_code;
}

/*
 * Returns the name of the food.
 */
string food_item::getName()
{
  return name;
}

/*
 * Returns the shelf life of the food.
 */
int food_item::getShelfLife()
{
  return shelf_life;
}




/* food_item.cc
 *
 * A food item has: 
 *  UPC Code (single string of numbers)
 *  Shelf life (number in days)
 *  name (a string of words) 
 */
#include <string>
#include "food_item.h"
// implementation
food_item::food_item(const string upc_code,int  shelf_life, const string name) {
  this->upc_code = upc_code;
  this->shelf_life = shelf_life;
  this->name = name;

}

/*
 * Subtracts one from the shelf life of the food. Stops when shelf life equals 0.
 */
void food_item::dec_shelf_life()
{
  if(shelf_life > 0)
    shelf_life--;
}
/*
 * Returns the upc code of the food.
 */
string food_item::get_UPC()
{
  return upc_code;
}

/*
 * Returns the name of the food.
 */
string food_item::get_name()
{
  return name;
}

/*
 * Returns the shelf life of the food.
 */
int food_item::get_shelf_life()
{
  return shelf_life;
}




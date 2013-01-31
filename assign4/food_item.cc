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
food_item::food_item(string item) {

  // Remove "FoodItem - UPC Code: " from the string.
  for(int i = 0, ws_counter = 0; i < item.length; i++)
    {
      if(item[i] = ' ')
	{
	++ws_counter;
	if(ws_counter == 4)
	  {
	    item = item.substr(i + 1, item.npos);
	  break;
	  }
	}
    }

  for(int i = 0; i < item.length; i++)
    {
      if(item[i] = ' ')
	{
	  this->upc_code = item.substr(0, i);
	  item = item.substr(i + 1, item.npos);
	  break;
	}
    }


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




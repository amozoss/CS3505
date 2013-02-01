/* food_item.cc
 *
 * A food item has: 
 *  UPC Code (single string of numbers)
 *  Shelf life (number in days)
 *  name (a string of words) 
 */
#include <string>
#include <cstdlib>
#include <iostream>
#include "food_item.h"
// implementation
food_item::food_item(string item) {

  // Remove "FoodItem - UPC Code: " from the string.
  // That is 4 spaces.
  for(int i = 0, ws_counter = 0; i < item.length(); i++)
  {
    if(item[i] == ' ')
    {

      if(ws_counter == 3)
      {
        item = item.substr(i + 1, item.npos);
        break;
      }
      ++ws_counter;
    }
  }

  // Get upc code and then remove it from item string.
  // String looks like this before for loop:
  //"0556467522  Shelf life: 15  Name: absolute vanilla vodka"
  for(int i = 0; i < item.length(); i++)
    {
      if(item[i] == ' ')
	{
	  this->upc_code = item.substr(0, i);
	  item = item.substr(i + 1, item.npos);
	  break;
	}
    }

  // String now looks like this:
  // " Shelf life: 15  Name: absolute vanilla vodka"
  // Remove leading space, space between "Shelf" and "life:"
  // and between that and "15". That is 3 spaces.
  for(int i = 0, ws_counter = 0; i < item.length(); i++)
    {
      if(item[i] == ' ')
	{
	if(ws_counter == 2)
	  {
	    item = item.substr(i + 1, item.npos);
	  break;
	  }
	++ws_counter;
	}
    }

  // Get the shelf life of the food and store it as an int.
  // Then remove the shelf life from the string. 
  // The string now looks like this:
  // "15  Name: absolute vanilla vodka"
  // This method will also remove the next character,
  // since it is also a whitespace.
  for(int i = 0; i < item.length(); i++)
    {
      if(item[i] == ' ' )
	{
	  this->shelf_life = atof(item.substr(0, i).c_str());
	  item = item.substr(i + 2, item.npos);
	  break;
	}
    }

  // Gets the name of the food and stores it
  // The string should look like this:
  // "Name: absolute vanilla vodka"
  for(int i = 0; i < item.length(); i++)
    {
      if(item[i] == ' ')
       {
	 this->name = item.substr(i + 1, item.npos);
	 break;
       }
    }
  /*cout << this->shelf_life << " is the shelf life.\n";
  cout << this->name << " is the name.\n";
  cout << this->upc_code << " is the UPC code.\n";
*/
}

/*
 * No-parameter constructor for when a food_item is being initialized.
 */
food_item::food_item()
{

}

/*
 * Destructor for food_item class.
 */
food_item::~food_item()
{


}
/*
 * Subtracts one from the shelf life of the food. Stops when shelf life equals 0.
 */
void food_item::dec_shelf_life()
{
  if(this->shelf_life > 0)
    this->shelf_life--;
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




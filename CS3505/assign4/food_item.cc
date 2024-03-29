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
#include <sstream>
#include <algorithm>
#include <iterator>
#include <vector>
// implementation
/*
 * Pares the item string and constructs a food item
 */
food_item::food_item(string item) {
  // split the string
 istringstream iss(item); 
  vector<string> tokens;
  copy(istream_iterator<string>(iss),
      istream_iterator<string>(),
      back_inserter<vector<string> >(tokens));

  int loc = item.find( "Name:", 0 );

  // Get the name
  if( loc != string::npos ) {
    this->name = item.substr(loc+6, string::npos);
  }

  this->shelf_life = atoi(tokens[7].c_str());
  this->upc_code = tokens[4];
/*  cout << this->shelf_life << " is the shelf life.\n";
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




/* 
 * Warehouse file comment
 * warehouse.cc
 *
 * Dan Willoughby and Michael Banks
 */
#include "warehouse.h"
#include "transaction.h"
#include <string>
#include <iostream>
#include <sstream>
#include <map>
#include <cstdlib>
#include <sstream>
#include <algorithm>
#include <iterator>
#include <vector>


using namespace std;

/*
 * Constructs a warehouse using the line from the data text 
 * and parses the name.
 */
warehouse::warehouse(string warehouse_data, map<string, food_item> food_map){
  
  istringstream iss(warehouse_data); 
  vector<string> tokens;
  copy(istream_iterator<string>(iss),
       istream_iterator<string>(),
       back_inserter<vector<string> >(tokens));
  
  foods = food_map;
  this->name = tokens[2];
  
}

warehouse::warehouse() 
  :name(""), effective_date("01/01/2011") 
{

}

warehouse::~warehouse()
{

}

/*
 * Is sent items that have been recieved/requested
 * by this warehouse.
 */
void warehouse::add_transaction(string trans)
{
  // int s_l = 2;
  transaction r(trans,this->effective_date.to_str());
  map<string,food_item>::iterator iter = foods.find(r.get_upc_code());
  if(iter != foods.end())
  {
    food_item food = iter->second;
    r.set_shelf_life(food.get_shelf_life());
  }

  map<string,int>::iterator it = food_inventory.find(r.get_upc_code());
  if(it != food_inventory.end())
  {
    //  element found update quantity
    //  receive adds to quantity, request subtracts
    //  if the receive transaction shelf life is zero it is expired and should not be added to the 
    //  total quantity
    if (r.get_type() == transaction::receive && r.get_shelf_life() != 0) 
      it->second += r.get_quantity();
    else {
      it->second -= r.get_quantity();

      if (it->second <= 0) // if the quantity falls to zero remove food from inventory
        food_inventory.erase(it); 
    }
  }
  // element not found, insert food into inventory if its a receive transaction 
  else {
    if (r.get_type() == transaction::receive)
      food_inventory.insert( pair<string,int>(r.get_upc_code(),r.get_quantity()));
  }

  trans_list.push_back(r);
}

/*
 * Returns name, date and transaction quantity of busiest day.
 *
 * Goes through all transactions by date and determines which date
 * had the most transactions. It returns the transaction quantity.
 * If two or more days have the same amount of transactions it returns 
 * the first one.
 */
string warehouse::report_busiest_day()
{


}

/*
 * At the end of the reporting period this function receives 
 * a list of all the foods and checks to see whether its own list
 * has a deficit.  If a certain item is not in stock it will be added to
 * the list that is being returned.
 */
set<string> warehouse::report_food_deficit()
{

}

/* 
 * Returns warehouse's name
 */
string warehouse::get_name()
{
  return name;
}
/*
 * At the end of the reporting period this function receives 
 * a list of all the foods and checks to see whether its own list
 * has a surplus.  If a certain item is in stock it will be added to
 * the list that is being returned.
 */
set<string>  warehouse::report_foods_in_stock()
{
  set<string> s;
  for(map<string, int>::iterator iterator = food_inventory.begin(); iterator != food_inventory.end(); iterator++) {
    cout << iterator->first << " : " << iterator->second << endl;
    if (iterator->second > 0)
      s.insert(iterator->first);
  }

  return s;

}

/*
 * This function is called when it is the next day.
 */
void warehouse::forward_date(){
  for(iter = trans_list.begin(); iter != trans_list.end(); iter++)
    {
      (*iter).dec_shelf_life();
    }

  this->effective_date.next_date();

 
}

/* 
 * Sets the effective date
 */
void warehouse::set_start_date(string date) 
{
  cout << "setL " << date << endl;
  this->effective_date = easy_date(date);
}


string warehouse::convert_int_to_str(int the_number)
{
  stringstream ss;
  ss << the_number;
  string s = ss.str();
  return s;
}


string warehouse::convert_char_to_str(char the_char)
{
 stringstream ss;
  ss << the_char;
  string s = ss.str();
  return s;

}

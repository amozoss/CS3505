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
#include <map>


using namespace std;

/*
 * Constructs a warehouse using the line from the data text 
 * and parses the name.
 */
warehouse::warehouse(string warehouse_data, map<string, food_item> food_map){
  
  // Remove "Warehouse - " from the string.
  // That is 2 spaces.
  string warehouse_name;
  for(int i = 0, ws_counter = 0; i < warehouse_data.length(); i++)
  {
    if(warehouse_data[i] == ' ')
    {
      ws_counter++;
      if(ws_counter == 2)
      {
        warehouse_name = warehouse_data.substr(i + 1, warehouse_data.npos);
//        cout << "warehouse: " << warehouse_name << endl;
        break;
      }
    }
  }

  foods = food_map;
  this->name = warehouse_name;

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
  map<string,food_item>::iterator it = foods.find(r.get_upc_code());
  if(it != foods.end())
  {
    food_item food = it->second;
    r.set_shelf_life(food.get_shelf_life());
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
set<string>  warehouse::report_foods_in_stock(){



}


/*
 * This function is called when it is the next day.
 */
void warehouse::forward_date(){
  // Decrement shelf life
  // Forward food date.
  // 
}

/* 
 * Sets the effective date
 */
void warehouse::set_start_date(string date) 
{
  cout << "setL " << date << endl;
  this->effective_date = easy_date(date);
}






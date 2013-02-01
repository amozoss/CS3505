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
  int s_l = 2;
 
  transaction r(trans,this->effective_date.to_str());
  r.set_shelf_life(5);


  // insert food item in inventory or remove from inventory
  map<string,int>::iterator it = food_inventory.find(r.get_upc_code());
  if(it != food_inventory.end())
  {
       //element found update quantity
       if (r.get_type() == transaction::receive) 
         it->second += r.get_quantity();
       else {
         it->second -= r.get_quantity();

         if (it->second <= 0) // if the quantity falls to zero remove food from inventory
          food_inventory.erase(it); 
       }
  }
  else {// insert it
    if (r.get_type() == transaction::receive)
      food_inventory.insert( pair<string,int>(r.get_upc_code(),r.get_quantity()));
  }
 
  trans_list.push_back(r);

  for(iter = trans_list.begin(); iter != trans_list.end(); iter++)
    {
      transaction t = *iter;
      // cout << t.get_date() << " " << t.get_quantity() << " " << t.get_upc_code() << " " << t.get_type() << " " <<t.get_shelf_life();
    }
  // cout << endl;
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

}

/* 
 * Sets the effective date
 */
void warehouse::set_start_date(string date) 
{
  cout << "setL " << date << endl;
  this->effective_date = easy_date(date);
}






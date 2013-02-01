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
warehouse::warehouse(string name, map<string, food_item> *food_set){
  foods = food_set;
  this->name = name;

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
 
  transaction r(trans,"this->effective_date.to_str()");
  r.set_shelf_life(5);
 
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
string warehouse::report_food_deficit()
{

}

/*
 * At the end of the reporting period this function receives 
 * a list of all the foods and checks to see whether its own list
 * has a surplus.  If a certain item is in stock it will be added to
 * the list that is being returned.
 */
list<food_item>  warehouse::report_foods_in_stock(){



}


/*
 * This function is called when it is the next day.
 */
void warehouse::forward_date(){

}

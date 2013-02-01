/* 
 * Warehouse file comment
 * warehouse.cc
 *
 * Dan Willoughby and Michael Banks
 */
#include "warehouse.h"
#include "transaction.h"
#include <string>


using namespace std;
warehouse::warehouse(string name, set<food_item> *food_set){
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
  iter = trans_list.end();
  trans_list.insert(iter, transaction(trans, this->effective_date.to_str()));
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
string warehouse::report_food_deficit(){

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

/* 
 * Warehouse file comment
 * warehouse.cc
 *
 * Dan Willoughby and Michael Banks
 */
#include "warehouse.h"


using namespace std;
warehouse::warehouse(string name, set<food_item> *foodSet){
  foods = foodSet;

}

warehouse::~warehouse(){

}

/*
 * Is sent items that have been received by the warehouse, 
 * adds them to the data structure of transactions.
 */
void warehouse::receive(string theReceive){


}

/*
 * Is sent items that have been requested by the warehouse, 
 * adds them to the data structure of transactions.
 */
void warehouse::request(string theRequest){


}

/*
 * Returns name, date and transaction quantity of busiest day.
 *
 * Goes through all transactions by date and determines which date
 * had the most transactions. It returns the transaction quantity.
 * If two or more days have the same amount of transactions it returns 
 * the first one.
 */
string warehouse::reportBusiestDay(){

}

/*
 * At the end of the reporting period this function receives 
 * a list of all the foods and checks to see whether its own list
 * has a deficit.  If a certain item is not in stock it will be added to
 * the list that is being returned.
 */
string warehouse::reportFoodDeficit(){

}

/*
 * At the end of the reporting period this function receives 
 * a list of all the foods and checks to see whether its own list
 * has a surplus.  If a certain item is in stock it will be added to
 * the list that is being returned.
 */
list<food_item>  warehouse::reportFoodsInStock(){



}


/*
 * This function is called when it is the next day.
 */
void warehouse::forwardDate(){

}

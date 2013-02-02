/* 
 * warehouse.cc
 *
 * A warehouse keeps manages transactions, shelf lives of food, and reports foods in stock
 * or foods in deficit
 *
 * Dan Willoughby and Michael Banks
 */
#include "warehouse.h"


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
  list<transaction> w;
  this->trans_list = w; 
}

warehouse::warehouse() 
  :name(""), effective_date("01/01/2011") 
{

}

/** Copy constructor:  Initialize this set
 *   to contain exactly the same elements as
 *   another set.
 */
warehouse::warehouse(const warehouse & other) 
{
    // Normal constructor tasks below.

    // Initialize this object to appear cleaned.
    this->food_inventory = other.food_inventory;
    this->name = other.name;
    this->effective_date = other.effective_date;
    this->foods = other.foods;
    this->trans_list = other.trans_list;

    // Use the overloaded assignment operator to do the work
    //   of copying the parameter's elements into this object.
    
    *this = other;
} // Copy constructor

warehouse::~warehouse()
{

}

/*
 * Is sent items that have been recieved/requested
 * The food inventory is managed as it goes
 *
 * by this warehouse.
 */
void warehouse::add_transaction(string trans)
{
  // make transaction
  transaction r(trans,this->effective_date.to_str());
  map<string,food_item>::iterator iter = foods.find(r.get_upc_code());
  if(iter != foods.end()) {
    food_item food = iter->second;
    r.set_shelf_life(food.get_shelf_life());
  }

  string k = r.get_upc_code();   // key
   int v = r.get_quantity();   // value

  map<string,int>::iterator lookup = food_inventory.find(r.get_upc_code());
  if(lookup != food_inventory.end())
  {
    // key already exists
    //  receive adds to quantity, request subtracts
    //  if the receive transaction shelf life is zero it is expired and should not be added to the 
    //  total quantity
   // cout << "found " << k << " in inv " << name << endl;
    if (r.get_type() == transaction::receive) {
    //  cout << "adding "  << v << endl;
      lookup->second += v;
    }
    else {
      lookup->second -= v;
      //cout << "subtracting " << v << endl;
      if (lookup->second <= 0) // if the quantity falls to zero remove food from inventory
        food_inventory.erase(lookup); 
    }
  }
  else
  {
    // the key does not exist in the map
    // add it to the map
   // cout << "didn't find "<< k << " in " << name << " " << transaction::receive << endl;
    if (r.get_type() == transaction::receive) {
      food_inventory.insert( pair<string,int>(k,v));
     // cout << "added item "<< k << " to " << this->name << " quant: " << v << endl;
    }
  }
  trans_list.push_back(r); // keep track of transaction
  //cout << "add: " << r.get_upc_code() << endl;
}

/*
 * Returns name, date and transaction quantity of busiest day.
 *
 * Goes through all transactions by date and determines which date
 * had the most transactions. It returns the transaction quantity.
 * If two or more days have the same amount of transactions it returns 
 * a later date.
 */
string warehouse::report_busiest_day()
{
  string current_date = "";//trans_list.front().get_date();
  string busiest_date = "";//trans_list.front().get_date();

  int c_quantity = 0;          // Current day's quantity.
  int b_quantity = 0;          // Busiest day's quantity.
  for(list<transaction>::iterator iter = trans_list.begin(); iter != trans_list.end(); iter++)
  {
    transaction t = *iter;

    cout << t.get_date() << " " << busiest_date << endl;
    // Check to see if date is the same as yesterday.
    // If it is, add the transactions quantity to the current day's quantity.
    if(current_date == t.get_date())
    {
    //  cout << "equals " << name << endl;
      c_quantity += t.get_quantity();
    }
    else
    {
      // If it isn't, check to see if c_quantity is greater than or equal to b_quantity.
      //    If it is, assign it to b_quantity and assign current date to busiest date.
      if(c_quantity >= b_quantity)
      {
   //     cout << "new busiest day " << busiest_date << endl;
        busiest_date = current_date;
        b_quantity = c_quantity;
      }
      current_date = t.get_date();
      c_quantity = 0;
    }
    
    cout << "___________" << endl;
  }
  return this->name + " " + busiest_date + " " + convert_int_to_str(c_quantity);
}

/*
 * At the end of the reporting period this function receives 
 * a list of all the foods and checks to see whether its own list
 * has a deficit.  If a certain item is not in stock it will be added to
 * the list that is being returned.
 */
set<string> warehouse::report_food_deficit()
{
  set<string> s1 = report_foods_in_stock();
  set<string> default_set;
  //and you want to get the set of elements that are in one but not the other, you can use std::set_difference:
  for(map<string, food_item>::iterator food_it = foods.begin(); food_it != foods.end(); food_it++)
  {
    //deficit_array[i] = (food_it->first);
    default_set.insert(food_it->first);
    //i++;
  }

 // for (set<string>::iterator it = s1.begin(); it != s1.end(); it++)
   // cout << "Inv of" << name << *it << endl;
  //for (set<string>::iterator it = default_set.begin(); it != default_set.end(); it++)
  //  cout << *it << endl;
  set<string> difference;
  //set_difference(s1.begin(), s1.end(),
    //  default_set.begin(), default_set.end(),
    //  inserter(difference, difference.begin()));
  set_difference(default_set.begin(), default_set.end(),
      s1.begin(), s1.end(),
      inserter(difference, difference.begin()));

 // for (set<string>::iterator it = difference.begin(); it != difference.end(); it++)
   // cout << "diff of" << name << *it << endl;
  return difference;
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

  for(map<string, int>::iterator iterator = food_inventory.begin(); iterator != food_inventory.end(); ++iterator) {
    s.insert(iterator->first);
  }
  return s;
}

/*
 * This function is called when it is the next day.
 */
void warehouse::forward_date(){

  for (list<transaction>::iterator iterator = trans_list.begin(), end = trans_list.end(); iterator != end; ++iterator) {
    //cout << (*iterator).get_upc_code() << " is the food in " << name << " with shelflife " << (*iterator).get_shelf_life() << " with quantity " << (*iterator).get_quantity() << endl;
    
    // if the shelf life goes to zero or below remove the food_item for inventory
    if ((*iterator).get_shelf_life() <= 0) { 
      map<string,int>::iterator lookup = food_inventory.find((*iterator).get_upc_code());
      if(lookup != food_inventory.end()) {
     //   cout << "erase: " << lookup->first << " in " << name << endl;
        food_inventory.erase(lookup);
      }
    }
    (*iterator).dec_shelf_life();
  }
  this->effective_date.next_date();
}

/* 
 * Sets the effective date
 */
void warehouse::set_start_date(string date) 
{
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

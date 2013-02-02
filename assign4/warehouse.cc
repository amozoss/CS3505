/* 
 * Warehouse file comment
 * warehouse.cc
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
    this->iter = other.iter;

    // Use the overloaded assignment operator to do the work
    //   of copying the parameter's elements into this object.
    
    *this = other;
} // Copy constructor

warehouse::~warehouse()
{

}

/*
 * Is sent items that have been recieved/requested
 * by this warehouse.
 */
void warehouse::add_transaction(string trans)
{
  transaction r(trans,this->effective_date.to_str());
  map<string,food_item>::iterator iter = foods.find(r.get_upc_code());
  if(iter != foods.end()) {
  
    food_item food = iter->second;
    r.set_shelf_life(food.get_shelf_life());
  //  cout << "found food with shelf life: " << food.get_shelf_life() << endl;
  }

  string k = r.get_upc_code();   // key
   int v = r.get_quantity();   // value

  //food_inventory[k]=v;
  //typedef map<string, food_item> MapType;    // Your map type may vary, just change the typedef
  //MapType::iterator lookup = this->food_inventory.lower_bound(k);

  map<string,int>::iterator lookup = food_inventory.find(r.get_upc_code());
  if(lookup != food_inventory.end())
  //if(lookup != food_inventory.end() && !(food_inventory.key_comp()(k, lookup->first)))
  {
    // key already exists
    // update lb->second if you care to
    //  receive adds to quantity, request subtracts
    //  if the receive transaction shelf life is zero it is expired and should not be added to the 
    //  total quantity
    cout << "found " << k << " in inv" << endl;
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
  //    cout << k <<" " << v << endl;
      food_inventory.insert( pair<string,int>(k,v));
     // cout << "added item "<< k << " to " << this->name << " quant: " << v << endl;
    }
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
  cout << "the warehouse: " << this->name << endl;
  for(map<string, int>::iterator iterator = food_inventory.begin(); iterator != food_inventory.end(); iterator++) {
    cout << iterator->first << " :reports_foods_in_stock: " << iterator->second << endl;
    //if (iterator->second == "4")
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

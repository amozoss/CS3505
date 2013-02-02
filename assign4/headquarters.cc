/*
 * headquarters.cc
 * Authors: Michael Banks and Dan Willoughby
 */
#include "headquarters.h"
#include <cstdlib>
#include <sstream>
#include <algorithm>
#include <iterator>
#include <vector>

headquarters::headquarters(string file_path)
{
  ifstream in(file_path.c_str());
  //ifstream in("word dog\n wahoo!");

  // Loop for reading the file.  Note that it is controlled
  //   from within the loop (see the 'break').

  while (true)
  {
    // Read a line 
    string line;
    getline(in,line);

    // If the read failed, we're probably at end of file
    //   (or else the disk went bad).  Exit the loop.
    if (in.fail()) {
      break;
    }

    // line  successfully read.  Add it to data

    //cout << line << endl;
    file_data.push_back(line);
  }
  in.close();

  read_data_lines();
  
}

/*
 * returns warehouse
 */
warehouse& headquarters::get_warehouse(string wh) 
{
  map<string,warehouse>::iterator iter = warehouses.find(wh);
 // if(iter != warehouses.end())
 // {
    return  iter->second;
   // cout << "found warehouse" << endl;
 // }
 // cout << "finished searching" << endl;
 // string s = "ji";
 // warehouse ww;
 // return ww;
}

string parse_start_date(string line)
{
 istringstream iss(line); 
  vector<string> tokens;
  copy(istream_iterator<string>(iss),
      istream_iterator<string>(),
      back_inserter<vector<string> >(tokens));
  return tokens[2];
}




string parse_warehouse_name(string line)
{
 istringstream iss(line); 
  vector<string> tokens;
  copy(istream_iterator<string>(iss),
      istream_iterator<string>(),
      back_inserter<vector<string> >(tokens));
  return tokens[3];
}

/* 
 * reads the data lines in file_data vector and calls class constructors
 */
void headquarters::read_data_lines () 
{
  
  for (list<string>::iterator it = file_data.begin(); it != file_data.end();it++)
  { // iterate vector

    string line = *it; // the current line being parsed
    string which_class;

    // Reads to the first white space.
    for(int i = 0; i < line.length(); i++)
    {
     // cout << line[i];
      if(line[i] == ' ')
      {
        which_class = line.substr(0,i); 
        break;
      }
    }
    if (which_class == "FoodItem") {
      food_item food(line); // use the class to parse the string, and then store them in the map,
      //cout << "Added food item: " <<food.get_name() << endl;
      food_items.insert ( pair<string,food_item>(food.get_UPC(),food) );
    }
    else if (which_class == "Warehouse") {
      //cout << line << endl;
      warehouse wh(line,food_items); // create warehouse and then map it
      string name = wh.get_name();
      //cout << "Added warehouse : " << name << endl;
      warehouses.insert ( pair<string,warehouse>(wh.get_name(),wh) );
      
    }
    else if (which_class == "Request:" || which_class == "Receive:") {
      warehouse &w = get_warehouse(parse_warehouse_name(line));
      w.add_transaction(line);
    }
    else if (which_class == "Start") {
      string start_date = parse_start_date(line);
      for(map<string, warehouse>::iterator iterator = warehouses.begin(); iterator != warehouses.end(); iterator++) {
        iterator->second.set_start_date(start_date);
      }
    }
    else if (which_class == "Next") {
      //cout << "----------next-----------" << endl;
      for(map<string, warehouse>::iterator iterator = warehouses.begin(); iterator != warehouses.end(); iterator++) {
        iterator->second.forward_date();
      }
    }

  }
}
    



headquarters::headquarters() {

}



headquarters::~headquarters()
{


}

/*
 * Gets the food_items that are in every warehouse
 */
set<string> headquarters::get_stocked_products(set<string> default_set)
{
  // get all the unstocked products in all warehouse, the difference is the stocked
  set<string> all_of_unstocked;
  for(map<string, warehouse>::iterator it = warehouses.begin();
      it != warehouses.end(); it++)
  {
    warehouse w = it->second;
    set<string> surplus = w.report_food_deficit();
    for( set<string>::iterator set_it = surplus.begin(); set_it != surplus.end(); set_it++)
    {
      all_of_unstocked.insert((*set_it));
    }
  }
  //for (set<string>::iterator it = all_of_stocked.begin(); it != all_of_stocked.end(); it++)
    //cout << "Inv of"  << *it << endl;
  set<string> difference;
  set_difference(default_set.begin(), default_set.end(),
      all_of_unstocked.begin(), all_of_unstocked.end(),
      inserter(difference, difference.begin()));
 // for (set<string>::iterator it = difference.begin(); it != difference.end(); it++)
   // cout << "diff of" << *it << endl;
  return difference;
}

/*
 * Gets the food items absent in every warehouse
 */
set<string> headquarters::get_unstocked_products(set<string> default_set)
{
  // get all the stocked products in all warehouse, the difference is the unstocked
  set<string> all_of_stocked;
  for(map<string, warehouse>::iterator it = warehouses.begin();
      it != warehouses.end(); it++)
  {
    warehouse w = it->second;
    set<string> w_deficit = w.report_foods_in_stock();
    for( set<string>::iterator set_it = w_deficit.begin(); set_it != w_deficit.end(); set_it++)
    {
      all_of_stocked.insert((*set_it));
    }
  }
  //for (set<string>::iterator it = all_of_stocked.begin(); it != all_of_stocked.end(); it++)
   // cout << "Inv of"  << *it << endl;
  set<string> difference;
  set_difference(default_set.begin(), default_set.end(),
      all_of_stocked.begin(), all_of_stocked.end(),
      inserter(difference, difference.begin()));
  return difference;
}


void headquarters::generate_report(){
  // Create a set of all the UPC codes.
  set<string> default_set;

  for(map<string, food_item>::iterator food_it = food_items.begin(); food_it != food_items.end(); food_it++) {
    default_set.insert(food_it->first);
  }

  /*On a single line, print out the title: "Report by " followed by your names.
   *Print a single blank line following the title.
   */
  cout << "Report by Dan Willoughby and Michael Banks" << endl << endl;

  // On a single line, print out "Unstocked Products:".
  cout << "Unstocked Products:" << endl;
  set<string> unstocked = get_unstocked_products(default_set);

  //Print the upc code and the name of the food.
  for( set<string>::iterator set_it = unstocked.begin(); set_it != unstocked.end(); set_it++)
  {
    cout << (*set_it) << " " << food_items[(*set_it)].get_name() << endl;
  }
  cout << endl;
  cout << "Fully-Stocked Products:" << endl;
  set<string> stocked = get_stocked_products(default_set);

  //Print the upc code and the name of the food.
  for( set<string>::iterator set_it = stocked.begin(); set_it != stocked.end(); set_it++)
  {
    cout << (*set_it) <<" " <<  food_items[(*set_it)].get_name() << endl;
  }

  /*
    Don't print out any other information, such as expiration dates, warehouse names, or quantities. 
    Just list the products (no duplicates) that are absent from every warehouse.

    Print a single blank line following the unstocked product list.

    Determine which products still exist in every warehouse.  On a single line, print out "Fully-Stocked 
    Products:".  On the following lines, print out a list of the products that have positive quantities 
    in every warehouse (in any order, no duplicates).  For each food item, only print out its UPC and name, as follows:

    Fully-Stocked Products:
    0984712812 mushroom ice cream
    0278374652 seaweed cereal

    To be clear, if a product has a positive quantity in only 9 out of 10 warehouses, it would not be on this list. 

    Print a single blank line following the fully-stocked product list.

    Determine the single busiest day for each warehouse.  The single busiest day for each warehouse is the day with the most products received and shipped (added together), with ties going to later days.  On a single line, print out "Busiest Days:".  On the following lines, print out one line for each warehouse.  Print out the warehouse name, the date (as MM/DD/YYYY), and the sum of all the transaction quantities for that day, as follows:

    Busiest Days:
    Miami 03/16/2005 19283
    Nome 12/24/2007 1827364
    Barstow 10/01/2006 12

*/


}

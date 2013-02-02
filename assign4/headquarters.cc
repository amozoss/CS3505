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
      cout << "Added food item: " <<food.get_name() << endl;
      food_items.insert ( pair<string,food_item>(food.get_UPC(),food) );
    }
    else if (which_class == "Warehouse") {
      //cout << line << endl;
      warehouse wh(line,food_items); // create warehouse and then map it
      string name = wh.get_name();
      cout << "Added warehouse : " << name << endl;
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

  }
}
    



headquarters::headquarters() {

}



headquarters::~headquarters()
{


}



void headquarters::generate_report(){
  /*On a single line, print out the title: "Report by " followed by your names.
   *Print a single blank line following the title.
   */
  cout << "Report by Dan Willoughby and Michael Banks" << endl << endl;

  // On a single line, print out "Unstocked Products:".
  cout << "Unstocked Products:" << endl;

  // Create a set of all the UPC codes.
  set<string> default_set;
  
  // for(int i = 0; i < food_items.size(); i++)
  //{
  //  default_set.insert(food_items[i].get_upc_code());
  //}

  //string * deficit_array = new string[food_items.size()];
  int i = 0;
  for(map<string, food_item>::iterator food_it = food_items.begin(); food_it != food_items.end(); food_it++)
    {
      //deficit_array[i] = (food_it->first);
      default_set.insert(food_it->first);
      //i++;
    }

  
  set<string> deficit_set = default_set;    // Will hold all upc codes that are out in a particular warehouse
  set<string> difference;
  map<string, warehouse>::iterator it = warehouses.begin();

  // Find the intersection between the default set and the set of elements that are missing.
  for(; it != warehouses.end(); it++)
    {
      warehouse w = it->second;
      set<string> out_of = w.report_food_deficit();
      std::set_intersection(deficit_set.begin(), deficit_set.end(), out_of.begin(), out_of.end(), inserter(deficit_set, deficit_set.begin()));
    }

  //Print the upc code and the name of the food.
  for( set<string>::iterator set_it = deficit_set.begin(); set_it != deficit_set.end(); set_it++)
    {
      cout << (*set_it) << food_items[(*set_it)].get_name() << endl;
    }


  cout << endl;
  cout << "Fully-Stocked Products:" << endl;
  set<string> surplus = default_set;

  for(it = warehouses.begin(); it != warehouses.end(); it++)
    {
      warehouse w = it->second;
      set<string> in_stock = w.report_foods_in_stock();
      std::set_intersection(surplus.begin(), surplus.end(), in_stock.begin(), in_stock.end(), inserter(surplus, surplus.begin()));
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

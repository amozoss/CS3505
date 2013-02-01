/*
 * headquarters.cc
 * Authors: Michael Banks and Dan Willoughby
 */
#include "headquarters.h"

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
warehouse headquarters::get_warehouse(string warehouse) 
{
  return warehouses.find(warehouse)->second;
}

string parse_start_date(string line)
{

  // Get the start date
  // That is 2 spaces.
  string date;
  for(int i = 0, ws_counter = 0; i < line.length(); i++)
  {
    if(line[i] == ' ')
    {
      ws_counter++;
      if(ws_counter == 2)
      {
        date = line.substr(i + 1, line.npos);
        cout << "start: " << date << endl;
        break;
      }
    }
  }
  return date;
}




string parse_warehouse_name(string line)
{

  // Get the name of the warehouse from transaction.
  // That is 3 spaces.
  string warehouse_name;
  for(int i = 0, ws_counter = 0; i < line.length(); i++)
  {
    if(line[i] == ' ')
    {
      ws_counter++;
      if(ws_counter == 3)
      {
        warehouse_name = line.substr(i + 1, line.npos);
        //        cout << "warehouse: " << warehouse_name << endl;
        break;
      }
    }
  }
  return warehouse_name;
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
      warehouse wh(line,food_items); // create warehouse and then map it
      cout << "Added warehouse : " << wh.get_name()<< endl;
      warehouses.insert ( pair<string,warehouse>(wh.get_name(),wh) );
      
    }
    else if (which_class == "Request:" || which_class == "Receive:") {
      warehouse w = get_warehouse(parse_warehouse_name(line));
      //cout << w.get_name() << endl;
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



  /*
    Determine which products do not exist in any warehouse..  On a single line, print out "Unstocked Products:".  On the following lines, print out a list of the products that do not exist in any warehouse (in any order, no duplicates).  For each food item, only print out its UPC and name, as follows:

    Unstocked Products:
    0984713912 pizza
    0278374752 bagels

    Don't print out any other information, such as expiration dates, warehouse names, or quantities.  Just list the products (no duplicates) that are absent from every warehouse.

    Print a single blank line following the unstocked product list.

    Determine which products still exist in every warehouse.  On a single line, print out "Fully-Stocked Products:".  On the following lines, print out a list of the products that have positive quantities in every warehouse (in any order, no duplicates).  For each food item, only print out its UPC and name, as follows:

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

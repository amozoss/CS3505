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
      
    }
    else if (which_class == "Request:" || which_class == "Receive:") {

    }
  }
}
    



headquarters::headquarters() {

}



headquarters::~headquarters()
{


}



void headquarters::generate_report(){


}

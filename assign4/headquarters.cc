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
    if (in.fail())
      break;

    // line  successfully read.  Add it to data

    cout << line << endl;
    file_data.insert(line);
  }


  in.close();
  
}

/* 
 * reads the data lines in file_data vector and calls class constructors
 */
void headquarters::read_data_lines () 
{
  for (int j = 0; j < file_data.size(); j++) { // iterate vector

    string line = file_data[j]; // the current line being parsed
    string parameter; // the string to be passed to the specific class constructor
    string which_class;

    // Reads to the first white space.
    for(int i = 0; i < s.length(); i++)
    {
      cout << line[i];
      if(line[i] == ' ')
      {
        which_class = line.substr(0,i); 
        parameter = line.substr(i + 1, s.npos);
        break;
      }
    }
    if (which_class == "FoodItem") {
      food_list.insert(food_item(parameter);
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

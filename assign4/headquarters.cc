/*
 * headquarters.cc
 * Authors: Michael Banks and Dan Willoughby
 */
#include "headquarters.h"

headquarters::headquarters(string file_path)
{
  ifstream in(file_path.c_str());

  // Loop for reading the file.  Note that it is controlled
  //   from within the loop (see the 'break').

  while (true)
  {
    // Read a line 
    string line;
    in >> line;

    // If the read failed, we're probably at end of file
    //   (or else the disk went bad).  Exit the loop.
    if (in.fail())
      break;

    // line  successfully read.  Add it to data

    cout << line << endl;
   // file_data.insert(line);
  }


  in.close();
  
}

headquarters::headquarters() {
}



headquarters::~headquarters()
{


}



void headquarters::generate_report(){


}

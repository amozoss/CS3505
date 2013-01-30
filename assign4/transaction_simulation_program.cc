/*
 * The transaction simulation program takes a filename as a command
 * line parameter and generates a report. 
 *
 * Authors: Dan Willoughby and Michael Banks
 *
 */


#include <iostream>
#include <string>
#include "headquarters.h"

using namespace std;

int main (int argc, char* argv[])
{
  // Check console parameter count
  if (argc != 2) {
    cout << "Invalid number of parameters.\n";
    return 0;
  } 
  string filepath = argv[1];
  headquarters head(filepath);
} 

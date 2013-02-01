/* headquarters.h
 *
 * Headquarters class generates reports
 * Authors: Michael Banks and Dan Willoughby
 *
 */

#ifndef HEADQUARTERS_HEADER_IKL
#define HEADQUARTERS_HEADER_IKL

#include <string>
#include <set>
#include <iostream>
#include <fstream>
#include <string>
#include <map>
#include <list>
#include "food_item.h"
#include "easy_date.h"
#include "warehouse.h"
#include "transaction.h"

using namespace std;

class headquarters
{
 public:
  headquarters(string file_path); 
  headquarters();
  
  ~headquarters();
  void generate_report ();
  warehouse get_warehouse(string);

 private:

  map<string, food_item> food_items;
  map<string, warehouse> warehouses;
  list<string> file_data; // stores all the lines in the data file
  void read_file(string file_path);
  void read_data_lines (); 
};
#endif

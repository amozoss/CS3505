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
#include <list>
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
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
  void generate_report();
  

 private:
  set<food_item> food_list;
  vector<string> file_data; // stores all the lines in the data file
  void read_file(string file_path);
};
#endif

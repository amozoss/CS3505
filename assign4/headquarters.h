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
#include "food_item.cc"
#include "easy_date.cc"
#include "warehouse.h"
#include "transaction.cc"

using namespace std;

class headquarters
{
 public:
  headquarters(string filePath); 
  ~headquarters();
  void generateReport();

 private:
 set<food_item> foodList;
};






#endif

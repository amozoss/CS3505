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
#include "food_item.h"
#include "easy_date.h"
#include "warehouse.h"
#include "transaction.h"

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

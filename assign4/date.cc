/*
 * Date Class Comment
 *
 *
 * Dan Willoughby and Michael Banks
 *
 */
#ifndef DATE_CLASS
#define DATE_CLASS

#include <string>

using namespace std;

class date {
public:

  date(string date);
private:
  string the_date;
};

#endif

// implementation
date::date(string date) 
  :date(date) {
    // this is the fancy custom date class

}

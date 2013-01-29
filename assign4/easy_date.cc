/*
 * Easy Date Class uses the boost library to keep track of dates. 
 *
 * The day can be incremented forward one day. 
 *
 *
 * Authors: Dan Willoughby and Michael Banks
 *
 */
#ifndef EASY_DATE_CLASS
#define EASY_DATE_CLASS

#include <string>
#include <iostream>
#include "boost/date_time/gregorian/gregorian.hpp" //include all types plus i/o
#include "boost/date_time/gregorian/parsers.hpp"
#include <locale>
#include <boost/date_time.hpp>
using std::locale;

using namespace boost::local_time;
using namespace boost::gregorian;

using namespace std;

class easy_date {
public:
  easy_date(string start_date); 
  string next_date();
private:
  date the_date;
};

#endif

// implementation

/* 
 * Constructs a date from the given start date string
 *
 * The format of the string must be MM/DD/YYYY. Ex. 05/01/2010
 */
easy_date::easy_date(string start_date)  {
  this->easy_date::the_date = from_uk_string(start_date);
  //cout << the_date << endl;
}

/*
 * Increments the date by one day
 *
 * If date is null, calls the start date constructor with current date
 */
string easy_date::next_date() {
  //Add duration to both begin and end.
  date_period dp(the_date, days(1));
  // dp == 2005-Jan-04 to 2005-Jan-05
} 

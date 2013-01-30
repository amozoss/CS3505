/*
 * Easy Date Class uses the boost library to keep track of dates. 
 *
 * The day can be incremented forward one day. 
 *
 *
 * Authors: Dan Willoughby and Michael Banks
 *
 */
#include "easy_date.h"

/* 
 * Constructs a date from the given start date string
 *
 * The format of the string must be MM/DD/YYYY. Ex. 05/01/2010
 */
easy_date::easy_date(std::string &startDate)  {
  date theCreatedDate = from_us_string(startDate);
  this->theDate = theCreatedDate;
  std::cout << theDate << std::endl;
}

/*
 * Increments the date by one day
 *
 * If date is null, calls the start date constructor with current date
 */
std::string easy_date::nextDate() {
  //Add duration to both begin and end.
  //date_period dp(theDate, days(1));
  // dp == 2005-Jan-04 to 2005-Jan-05
} 

std::string easy_date::to_str() {
//  date dt(from_string(ds));
//  date_facet *f=new date_facet("%Y-%m-%d");
 // std::string s(to_simple_string(theDate));

 // return s;
}
  

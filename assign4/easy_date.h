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
//#include "boost/date_time/gregorian/gregorian.hpp" //include all types plus i/o
//#include "boost/date_time/gregorian/parsers.hpp"
//#include <locale>
//#include <boost/date_time.hpp>
//using std::locale;

//using namespace boost::local_time;
//using namespace boost::gregorian;

//using namespace std;

class easy_date {
public:
  easy_date(std::string &start_date); 
  easy_date();
  std::string next_date();
  std::string to_str();
private:
int day;
int month;
int year;
  //date theDate;
};
#endif

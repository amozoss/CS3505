/*
 * Easy Date Class uses the boost library to keep track of dates. 
 *
 * The day can be incremented forward one day. 
 *
 *
 * Authors: Dan Willoughby and Michael Banks
 *
 */
#include <sstream>
#include <string>
#include <cstdlib>
#include "easy_date.h"

using namespace std;

easy_date::easy_date(string start_date)
{
  day = atof(start_date.substr(3, 2).c_str());
  month = atof(start_date.substr(0, 2).c_str());
  year = atof(start_date.substr(6, 4).c_str());
}

easy_date::easy_date()
{

}

easy_date::~easy_date()
{

}

string easy_date::next_date()
{
  int temp_date = day;
  day++;
  // The case where the month has 31 days but is not the last month of the year.
  if(month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10)
  {
    if(day > 31)
    {
      day = 1;
      month++;
    }
  }
  // The case where the month has 30 days.
  else if(month == 4 || month == 6 || month == 9 || month == 11)
  {
    if(day > 30)
    {
      day = 1;
      month++;
    }
  }
  // The special case where the month has 28 or 29 days,
  // depending on the year.
  else if(month == 2)
  {
    if(year % 4 == 0)
    {
      if(day > 29)
      {
        day = 1;
        month++;
      }
    }
    else if(day > 28)
    {
      day = 1;
      month++;
    }

  }
  // The special case where it is the last month of the year.
  else if(month == 12)
  {
    if(day > 31)
    {
      day = 1;
      month = 1;
      year++;
    }
  }
  return this->to_str();
}

string easy_date::to_str()
{
  return convert_int(month) + "/" + convert_int(day) + "/" + convert_int(year);
}


string easy_date::convert_int(int the_number)
{
  stringstream ss;
  ss << the_number;
  string s = ss.str();
  if(s.length() == 1)
    s = "0" + s;
  return s;
}




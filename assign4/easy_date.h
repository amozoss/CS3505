/*
 * Easy Date class receives a date and can increment that date by one day
 * at a time.
 * Authors: Michael Banks and Dan Willoughby
 *
 * Format for date is accepted as MM/DD/YYYY
 *
 */
#ifndef EASY_DATE_CLASS
#define EASY_DATE_CLASS
#include <iostream>
#include <string>

class easy_date
{
 public:
  easy_date(std::string start_date);
  easy_date();
  ~easy_date();
  std::string next_date();
  std::string to_str();

 private:
  std::string convert_int(int);
  int day;
  int month;
  int year;
};
#endif

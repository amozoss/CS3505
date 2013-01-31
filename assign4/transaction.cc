/*
 * transaction.cc
 * 
 * Authors: Michael Banks and Dan Willoughby
 *
 *
 */

#include "transaction.h"
#include <iostream>
#include <cstdlib>
#include <string>

transaction::transaction(string s, string date)
{
  // Determine if receive or request.
  if(s[2] == 'c')
    this->type_of_transaction = receive;
  else
    this->type_of_transaction = request;

  // Remove transaction type from string.
  for(int i = 0; i < s.length(); i++)
    {
      
    if(s[i] == ' ')
      {
      s = s.substr(i + 1, s.npos);
      break;
      }
    }


  // Assign upc code to upc_code
  for(int i = 0; i < s.length(); i++)
    {
    if(s[i] == ' ')
      {
      this->upc_code = s.substr(0, i);
      s = s.substr(i + 1, s.npos);
      break;
      }
    }


  





  // Assign quantity to quantity.
  for(int i = 0; i < s.length(); i++)
    if(s[i] == ' ')
      {
      this->quantity = atof(s.substr(0, i).c_str());
      break;
      }
  /*
  cout << this->upc_code << " is the UPC code." << endl;
  cout << this->quantity << " is the quantity." << endl;
  cout << "Transaction type " << this->type_of_transaction << endl;
  */

  the_date = date;
}

transaction::~transaction()
{

}



int transaction::get_type()
{
  return this->type_of_transaction;
}

string transaction::get_upc_code()
{
  return this->upc_code;
}

string transaction::get_date()
{
  return this->the_date;
}

int transaction::get_quantity()
{
  return this->quantity;
}

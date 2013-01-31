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

transaction::transaction(string s)
{
  // Determine if receive or request.
  if(s[2] == 'c')
    this->type_of_transaction = receive;
  else
    this->type_of_transaction = request;

  // Remove transaction type from string.
  for(int i = 0; i < s.length(); i++)
    {
      cout << s[i];
    if(s[i] == ' ')
      {
      s = s.substr(i + 1, s.npos);
      break;
      }
    }

  cout << " is the transaction type." << endl;

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


  cout << "Transaction type " << this->type_of_transaction << endl;



  cout << this->upc_code << " is the UPC code." << endl;

  // Assign quantity to quantity.
  for(int i = 0; i < s.length(); i++)
    if(s[i] == ' ')
      {
      this->quantity = atof(s.substr(0, i).c_str());
      break;
      }

  cout << this->quantity << " is the quantity." << endl;

  


}

transaction::~transaction()
{

}



int transaction::get_type()
{
  return type_of_transaction;
}

string transaction::get_upc_code()
{

}

string transaction::get_date()
{

}

int transaction::get_quantity()
{

}

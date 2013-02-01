/*
 * transaction.cc
 * 
 * Authors: Michael Banks and Dan Willoughby
 * A transaction has a UPC code, a date, a quantity, a shelf life
 * and a type of transaction.  A transaction is a record for a type
 * of product at a particular warehouse on a particular date.
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
      this->quantity = atoi(s.substr(0, i).c_str());
      break;
    }
  
/*
  cout << this->upc_code << " is the UPC code." << endl;
  cout << this->quantity << " is the quantity." << endl;
  cout << "Transaction type " << this->type_of_transaction << endl;
*/ 
  this->the_date = date;
}

transaction::~transaction()
{

}

/*
 * Returns the shelf life of the transaction.
 */
int transaction::get_shelf_life()
{
  return shelf_life;
}

/*
 * Sets the shelf-life to the specified length.
 */
void transaction::set_shelf_life(int the_shelf_life)
{
  this->shelf_life = the_shelf_life;
}

void transaction::dec_shelf_life()
{
  if(this->shelf_life > 0)
    this->shelf_life--;
}

int transaction::get_type()
{
  return type_of_transaction;
}

string transaction::get_upc_code()
{
  return upc_code;
}

string transaction::get_date()
{
  return the_date;
}

int transaction::get_quantity()
{
  return quantity;
}

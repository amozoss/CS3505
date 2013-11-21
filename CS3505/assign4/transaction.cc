/*
 * transaction.cc
 * 
 * A transaction has a UPC code, a date, a quantity, a shelf life
 * and a type of transaction.  A transaction is a record for a type
 * of product at a particular warehouse on a particular date.
 *
 * Authors: Michael Banks and Dan Willoughby
 */
#include "transaction.h"
#include <iostream>
#include <cstdlib>
#include <string>
#include <sstream>
#include <algorithm>
#include <iterator>
#include <vector>

/*
 * Constructs a transaction from the string with date
 */
transaction::transaction(string s, string date)
{
  // parse string
  istringstream iss(s); 
  vector<string> tokens;
  copy(istream_iterator<string>(iss),
      istream_iterator<string>(),
      back_inserter<vector<string> >(tokens));

  // Determine if receive or request.
  if(tokens[0][2] == 'c')
    this->type_of_transaction = receive;
  else
    this->type_of_transaction = request;

  this->upc_code = tokens[1];
  this->quantity = atoi(tokens[2].c_str());
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
 * Sets the shelf life to the specified length.
 */
void transaction::set_shelf_life(int the_shelf_life)
{
  this->shelf_life = the_shelf_life;
}

/*
 * Decrements the shelf life of the food, if the shelf life
 * is zero it will not go any lower.
 */
void transaction::dec_shelf_life()
{
  if(this->shelf_life > 0)
    this->shelf_life--;
}

/*
 * Returns the type of transaction: a receive or request.
 */
int transaction::get_type()
{
  return type_of_transaction;
}

/*
 * Returns the UPC code of the transaction's food item.
 */
string transaction::get_upc_code()
{
  return upc_code;
}

/*
 * Returns the date that the transaction was executed.
 */
string transaction::get_date()
{
  return the_date;
}

/*
 * Returns how many products were received or requested
 * in this transaction.
 */
int transaction::get_quantity()
{
  return quantity;
}
